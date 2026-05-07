using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Integration.Test.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Integration.Test.Lifecycle
{
    /// <summary>
    /// Integration tests for role assignment revocation and session cascade (TASK-6.3).
    /// Covers: RevokedAt + RevokedByUserId set on revocation, revoked assignments excluded
    /// from active queries, and session revocation scoped to correct TenantId + CompanyId.
    /// </summary>
    public class RoleAssignmentRevocationTests
    {
        // ─── RevokedAt / RevokedByUserId ───────────────────────────────────────

        [Fact]
        public async Task RevokedAssignment_HasRevokedAtAndRevokedByUserId_Set()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId  = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var userId    = Guid.NewGuid();
            var actorId   = Guid.NewGuid();
            var roleId    = Guid.NewGuid();

            var assignment = new UserApplicationRole
            {
                Id               = Guid.NewGuid(),
                UserId           = userId,
                TenantId         = tenantId,
                CompanyId        = companyId,
                ApplicationId    = "app1",
                RoleId           = roleId,
                Status           = LifecycleStatus.RoleAssignment.Active,
                AssignedAt       = DateTime.UtcNow.AddDays(-1),
                AssignedByUserId = actorId,
            };
            db.UserApplicationRoles.Add(assignment);
            await db.SaveChangesAsync();

            var before = DateTime.UtcNow;

            // Simulate what LifecycleApiController.PatchRoleAssignmentStatus does
            assignment.Status           = LifecycleStatus.RoleAssignment.Revoked;
            assignment.RevokedAt        = DateTime.UtcNow;
            assignment.RevokedByUserId  = actorId;
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.RevokeRoleAssignmentAsync(userId, tenantId, companyId);

            var persisted = await db.UserApplicationRoles.FindAsync(assignment.Id);
            Assert.Equal(LifecycleStatus.RoleAssignment.Revoked, persisted!.Status);
            Assert.NotNull(persisted.RevokedAt);
            Assert.True(persisted.RevokedAt >= before);
            Assert.Equal(actorId, persisted.RevokedByUserId);
        }

        // ─── Revoked assignment excluded from active queries ───────────────────

        [Fact]
        public async Task ActiveQuery_ExcludesRevokedAssignments()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId  = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var userId    = Guid.NewGuid();
            var actorId   = Guid.NewGuid();

            db.UserApplicationRoles.AddRange(
                new UserApplicationRole { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId, ApplicationId = "app1", RoleId = Guid.NewGuid(), Status = LifecycleStatus.RoleAssignment.Active,  AssignedAt = DateTime.UtcNow, AssignedByUserId = actorId },
                new UserApplicationRole { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId, ApplicationId = "app1", RoleId = Guid.NewGuid(), Status = LifecycleStatus.RoleAssignment.Revoked, AssignedAt = DateTime.UtcNow, AssignedByUserId = actorId }
            );
            await db.SaveChangesAsync();

            var active = await db.UserApplicationRoles
                .Where(r => r.UserId == userId && r.Status == LifecycleStatus.RoleAssignment.Active)
                .ToListAsync();

            Assert.Single(active);
            Assert.All(active, r => Assert.Equal(LifecycleStatus.RoleAssignment.Active, r.Status));
        }

        // ─── Session revocation scoped to TenantId + CompanyId ────────────────

        [Fact]
        public async Task RevokeRoleAssignmentAsync_OnlyRevokes_SessionsScopedToTenantAndCompany()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId        = Guid.NewGuid();
            var companyId       = Guid.NewGuid();
            var otherCompanyId  = Guid.NewGuid();
            var userId          = Guid.NewGuid();

            // Target session: same user, same tenant, same company
            var targetSession = new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId,      Status = "Active", ClientId = "target",       ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow };
            // Different company: should not be revoked
            var otherCompany  = new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = otherCompanyId, Status = "Active", ClientId = "other-company", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow };
            // Different user: should not be revoked
            var otherUser     = new WorkspaceSession { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), TenantId = tenantId, CompanyId = companyId, Status = "Active", ClientId = "other-user", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow };

            db.WorkspaceSessions.AddRange(targetSession, otherCompany, otherUser);
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.RevokeRoleAssignmentAsync(userId, tenantId, companyId);

            var sessions = await db.WorkspaceSessions.ToListAsync();
            Assert.Equal("Revoked", sessions.First(s => s.ClientId == "target").Status);
            Assert.Equal("Active",  sessions.First(s => s.ClientId == "other-company").Status);
            Assert.Equal("Active",  sessions.First(s => s.ClientId == "other-user").Status);
        }

        [Fact]
        public async Task RevokeRoleAssignmentAsync_DoesNotRevoke_AlreadyRevokedSessions()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId  = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var userId    = Guid.NewGuid();

            db.WorkspaceSessions.Add(new WorkspaceSession
            {
                Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId,
                Status = "Revoked", ClientId = "already-revoked", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            // Must not throw — already-Revoked session is excluded by the WHERE Status='Active' filter
            await svc.RevokeRoleAssignmentAsync(userId, tenantId, companyId);

            var session = await db.WorkspaceSessions.FirstAsync(s => s.ClientId == "already-revoked");
            Assert.Equal("Revoked", session.Status);
        }

        [Fact]
        public async Task RevokeRoleAssignmentAsync_MultipleActiveSessions_AllRevoked()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var tenantId  = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var userId    = Guid.NewGuid();

            db.WorkspaceSessions.AddRange(
                new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId, Status = "Active", ClientId = "s1", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow },
                new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId, Status = "Active", ClientId = "s2", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow },
                new WorkspaceSession { Id = Guid.NewGuid(), UserId = userId, TenantId = tenantId, CompanyId = companyId, Status = "Active", ClientId = "s3", ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();

            var svc = new LifecycleCascadeService(db);
            await svc.RevokeRoleAssignmentAsync(userId, tenantId, companyId);

            var sessions = await db.WorkspaceSessions.Where(s => s.UserId == userId).ToListAsync();
            Assert.All(sessions, s => Assert.Equal("Revoked", s.Status));
        }

        // ─── Guard: RoleAssignment Revoked is terminal ─────────────────────────

        [Fact]
        public void Guard_RoleAssignment_Revoked_IsTerminal()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("RoleAssignment", LifecycleStatus.RoleAssignment.Revoked, LifecycleStatus.RoleAssignment.Active);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Guard_RoleAssignment_Active_CanBeRevoked()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("RoleAssignment", LifecycleStatus.RoleAssignment.Active, LifecycleStatus.RoleAssignment.Revoked);
            Assert.True(result.IsValid);
        }
    }
}
