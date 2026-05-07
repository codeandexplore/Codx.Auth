using Codx.Auth.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Infrastructure.Lifecycle
{
    /// <summary>
    /// EF Core implementation of <see cref="ILifecycleCascadeService"/>.
    /// Every method executes its cascade writes inside a single database transaction.
    /// </summary>
    public class LifecycleCascadeService : ILifecycleCascadeService
    {
        private readonly UserDbContext _context;

        public LifecycleCascadeService(UserDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task CancelTenantAsync(Guid tenantId, Guid actorUserId, CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // 1. Cancel all Companies in the tenant
            var companies = await _context.Companies
                .Where(c => c.TenantId == tenantId && c.Status != LifecycleStatus.Company.Cancelled)
                .ToListAsync(ct);
            foreach (var c in companies)
                c.Status = LifecycleStatus.Company.Cancelled;

            // 2. Remove all active/suspended Memberships in the tenant
            var memberships = await _context.UserMemberships
                .Where(m => m.TenantId == tenantId && m.Status != LifecycleStatus.Membership.Removed)
                .ToListAsync(ct);
            foreach (var m in memberships)
            {
                m.Status = LifecycleStatus.Membership.Removed;
                m.StatusChangedAt = DateTime.UtcNow;
                m.StatusChangedBy = actorUserId;
            }

            // 3. Remove all MembershipRoles under those memberships
            var membershipIds = memberships.Select(m => m.Id).ToList();
            var membershipRoles = await _context.UserMembershipRoles
                .Where(mr => membershipIds.Contains(mr.MembershipId) && mr.Status != LifecycleStatus.MembershipRole.Removed)
                .ToListAsync(ct);
            foreach (var mr in membershipRoles)
            {
                mr.Status = LifecycleStatus.MembershipRole.Removed;
                mr.StatusChangedAt = DateTime.UtcNow;
                mr.StatusChangedBy = actorUserId;
            }

            // 4. Revoke all Pending invitations in the tenant
            var invitations = await _context.Invitations
                .Where(i => i.TenantId == tenantId && i.Status == "Pending")
                .ToListAsync(ct);
            foreach (var inv in invitations)
                inv.Status = "Revoked";

            // 5. Revoke all active WorkspaceSessions in the tenant
            var sessions = await _context.WorkspaceSessions
                .Where(s => s.TenantId == tenantId && s.Status == "Active")
                .ToListAsync(ct);
            foreach (var s in sessions)
                s.Status = "Revoked";

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        /// <inheritdoc/>
        public async Task SuspendTenantAsync(Guid tenantId, CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // Revoke all active WorkspaceSessions for the tenant — record statuses unchanged
            var sessions = await _context.WorkspaceSessions
                .Where(s => s.TenantId == tenantId && s.Status == "Active")
                .ToListAsync(ct);
            foreach (var s in sessions)
                s.Status = "Revoked";

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        /// <inheritdoc/>
        public async Task CancelCompanyAsync(Guid companyId, Guid actorUserId, CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // 1. Remove Memberships scoped to this company
            var memberships = await _context.UserMemberships
                .Where(m => m.CompanyId == companyId && m.Status != LifecycleStatus.Membership.Removed)
                .ToListAsync(ct);
            foreach (var m in memberships)
            {
                m.Status = LifecycleStatus.Membership.Removed;
                m.StatusChangedAt = DateTime.UtcNow;
                m.StatusChangedBy = actorUserId;
            }

            // 2. Remove their MembershipRoles
            var membershipIds = memberships.Select(m => m.Id).ToList();
            var membershipRoles = await _context.UserMembershipRoles
                .Where(mr => membershipIds.Contains(mr.MembershipId) && mr.Status != LifecycleStatus.MembershipRole.Removed)
                .ToListAsync(ct);
            foreach (var mr in membershipRoles)
            {
                mr.Status = LifecycleStatus.MembershipRole.Removed;
                mr.StatusChangedAt = DateTime.UtcNow;
                mr.StatusChangedBy = actorUserId;
            }

            // 3. Revoke all active WorkspaceSessions for the company
            var sessions = await _context.WorkspaceSessions
                .Where(s => s.CompanyId == companyId && s.Status == "Active")
                .ToListAsync(ct);
            foreach (var s in sessions)
                s.Status = "Revoked";

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        /// <inheritdoc/>
        public async Task SuspendCompanyAsync(Guid companyId, CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // Revoke all active WorkspaceSessions for the company — record statuses unchanged
            var sessions = await _context.WorkspaceSessions
                .Where(s => s.CompanyId == companyId && s.Status == "Active")
                .ToListAsync(ct);
            foreach (var s in sessions)
                s.Status = "Revoked";

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        /// <inheritdoc/>
        public async Task RevokeRoleAssignmentAsync(Guid userId, Guid tenantId, Guid companyId, CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // Revoke all active WorkspaceSessions for the user scoped to (TenantId, CompanyId)
            var sessions = await _context.WorkspaceSessions
                .Where(s => s.UserId == userId
                         && s.TenantId == tenantId
                         && s.CompanyId == companyId
                         && s.Status == "Active")
                .ToListAsync(ct);
            foreach (var s in sessions)
                s.Status = "Revoked";

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
    }
}

