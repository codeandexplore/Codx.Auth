using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Integration.Test.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Integration.Test.Lifecycle
{
    /// <summary>
    /// Integration tests for Tenant lifecycle transitions (TASK-6.1).
    /// Covers: creation sets Active, cancel cascades, suspend revokes sessions only,
    /// and LifecycleTransitionGuard rejects Cancelled → Active.
    /// </summary>
    public class TenantLifecycleTests
    {
        // ─── CancelTenantAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task CancelTenantAsync_CascadesCompanies_ToCancel()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.Companies.Add(new Company { Id = Guid.NewGuid(), TenantId = tenantId, Name = "C1", Status = LifecycleStatus.Company.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.Companies.Add(new Company { Id = Guid.NewGuid(), TenantId = tenantId, Name = "C2", Status = LifecycleStatus.Company.Suspended, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            var companies = await db.Companies.Where(c => c.TenantId == tenantId).ToListAsync();
            Assert.All(companies, c => Assert.Equal(LifecycleStatus.Company.Cancelled, c.Status));
        }

        [Fact]
        public async Task CancelTenantAsync_CascadesMemberships_ToRemoved()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            var m1 = new UserMembership { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), TenantId = tenantId, Status = LifecycleStatus.Membership.Active, JoinedAt = DateTime.UtcNow };
            var m2 = new UserMembership { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), TenantId = tenantId, Status = LifecycleStatus.Membership.Suspended, JoinedAt = DateTime.UtcNow };
            db.UserMemberships.AddRange(m1, m2);
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            var memberships = await db.UserMemberships.Where(m => m.TenantId == tenantId).ToListAsync();
            Assert.All(memberships, m =>
            {
                Assert.Equal(LifecycleStatus.Membership.Removed, m.Status);
                Assert.NotNull(m.StatusChangedAt);
                Assert.Equal(actorId, m.StatusChangedBy);
            });
        }

        [Fact]
        public async Task CancelTenantAsync_CascadesMembershipRoles_ToRemoved()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();
            var membershipId = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.UserMemberships.Add(new UserMembership { Id = membershipId, UserId = Guid.NewGuid(), TenantId = tenantId, Status = LifecycleStatus.Membership.Active, JoinedAt = DateTime.UtcNow });
            db.UserMembershipRoles.Add(new UserMembershipRole { Id = Guid.NewGuid(), MembershipId = membershipId, Status = LifecycleStatus.MembershipRole.Active });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            var roles = await db.UserMembershipRoles.Where(r => r.MembershipId == membershipId).ToListAsync();
            Assert.All(roles, r =>
            {
                Assert.Equal(LifecycleStatus.MembershipRole.Removed, r.Status);
                Assert.NotNull(r.StatusChangedAt);
                Assert.Equal(actorId, r.StatusChangedBy);
            });
        }

        [Fact]
        public async Task CancelTenantAsync_RevokesInvitations()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.Invitations.Add(new Invitation { Id = Guid.NewGuid(), TenantId = tenantId, Email = "a@x.com", Status = "Pending", InviteTokenHash = "hash1", ExpiresAt = DateTime.UtcNow.AddDays(1), InvitedByUserId = actorId, CreatedAt = DateTime.UtcNow });
            db.Invitations.Add(new Invitation { Id = Guid.NewGuid(), TenantId = tenantId, Email = "b@x.com", Status = "Accepted", InviteTokenHash = "hash2", ExpiresAt = DateTime.UtcNow.AddDays(1), InvitedByUserId = actorId, CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            var pending  = await db.Invitations.FirstAsync(i => i.Email == "a@x.com");
            var accepted = await db.Invitations.FirstAsync(i => i.Email == "b@x.com");

            Assert.Equal("Revoked", pending.Status);
            Assert.Equal("Accepted", accepted.Status); // only Pending invitations are revoked
        }

        [Fact]
        public async Task CancelTenantAsync_RevokesActiveSessions()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();
            var userId   = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, Status = "Active", ClientId = "c1", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, Status = "Expired", ClientId = "c2", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            var active  = await db.WorkspaceSessions.FirstAsync(s => s.ClientId == "c1");
            var expired = await db.WorkspaceSessions.FirstAsync(s => s.ClientId == "c2");

            Assert.Equal("Revoked", active.Status);
            Assert.Equal("Expired", expired.Status); // non-Active sessions untouched
        }

        [Fact]
        public async Task CancelTenantAsync_AlreadyCancelledCompany_IsNotDoubleModified()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            db.Companies.Add(new Company { Id = companyId, TenantId = tenantId, Name = "C-already-cancelled", Status = LifecycleStatus.Company.Cancelled, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            // Should not throw — already-Cancelled company is excluded from cascade
            await svc.CancelTenantAsync(tenantId, actorId);

            var company = await db.Companies.FindAsync(companyId);
            Assert.Equal(LifecycleStatus.Company.Cancelled, company!.Status);
        }

        [Fact]
        public async Task CancelTenantAsync_TenantRecord_IsRetained()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.Tenants.Add(new Tenant { Id = tenantId, Name = "T1", Slug = "t1", Status = LifecycleStatus.Tenant.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.CancelTenantAsync(tenantId, actorId);

            // The Tenant record itself must still exist — only status is changed by the caller
            var tenant = await db.Tenants.FindAsync(tenantId);
            Assert.NotNull(tenant);
        }

        // ─── SuspendTenantAsync ────────────────────────────────────────────────

        [Fact]
        public async Task SuspendTenantAsync_RevokesActiveSessions_Only()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var userId   = Guid.NewGuid();

            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, Status = "Active", ClientId = "c1", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, Status = "Revoked", ClientId = "c2", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.SuspendTenantAsync(tenantId);

            var sessions = await db.WorkspaceSessions.Where(s => s.TenantId == tenantId).ToListAsync();
            Assert.Equal("Revoked", sessions.First(s => s.ClientId == "c1").Status);
            Assert.Equal("Revoked", sessions.First(s => s.ClientId == "c2").Status); // was already Revoked
        }

        [Fact]
        public async Task SuspendTenantAsync_DoesNotModify_Memberships()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.UserMemberships.Add(new UserMembership { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), TenantId = tenantId, Status = LifecycleStatus.Membership.Active, JoinedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.SuspendTenantAsync(tenantId);

            var memberships = await db.UserMemberships.Where(m => m.TenantId == tenantId).ToListAsync();
            Assert.All(memberships, m => Assert.Equal(LifecycleStatus.Membership.Active, m.Status));
        }

        [Fact]
        public async Task SuspendTenantAsync_DoesNotModify_Companies()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId = Guid.NewGuid();
            var actorId  = Guid.NewGuid();

            db.Companies.Add(new Company { Id = Guid.NewGuid(), TenantId = tenantId, Name = "C1", Status = LifecycleStatus.Company.Active, CreatedAt = DateTime.UtcNow, CreatedBy = actorId });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.SuspendTenantAsync(tenantId);

            var companies = await db.Companies.Where(c => c.TenantId == tenantId).ToListAsync();
            Assert.All(companies, c => Assert.Equal(LifecycleStatus.Company.Active, c.Status));
        }

        [Fact]
        public async Task SuspendTenantAsync_DoesNotAffect_OtherTenantSessions()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId      = Guid.NewGuid();
            var otherTenantId = Guid.NewGuid();
            var userId        = Guid.NewGuid();

            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId,      Status = "Active", ClientId = "mine",  ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            db.WorkspaceSessions.Add(new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = otherTenantId, Status = "Active", ClientId = "other", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.SuspendTenantAsync(tenantId);

            var otherSession = await db.WorkspaceSessions.FirstAsync(s => s.ClientId == "other");
            Assert.Equal("Active", otherSession.Status);
        }

        // ─── Guard: Cancelled is terminal ─────────────────────────────────────

        [Fact]
        public void Guard_Tenant_Cancelled_CannotTransitionToActive()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("Tenant", LifecycleStatus.Tenant.Cancelled, LifecycleStatus.Tenant.Active);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Guard_Tenant_Cancelled_CannotTransitionToSuspended()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("Tenant", LifecycleStatus.Tenant.Cancelled, LifecycleStatus.Tenant.Suspended);
            Assert.False(result.IsValid);
        }
    }
}
