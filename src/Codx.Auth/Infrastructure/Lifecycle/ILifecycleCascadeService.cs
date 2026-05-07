using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Infrastructure.Lifecycle
{
    /// <summary>
    /// Encapsulates all multi-entity cascade operations triggered by lifecycle status changes.
    /// Every method executes atomically within a single database transaction.
    /// </summary>
    public interface ILifecycleCascadeService
    {
        /// <summary>
        /// Cancels a tenant: cascades Companies → Cancelled, UserMemberships → Removed,
        /// UserMembershipRoles → Removed, pending Invitations → Revoked,
        /// and active WorkspaceSessions → Revoked.
        /// </summary>
        Task CancelTenantAsync(Guid tenantId, Guid actorUserId, CancellationToken ct = default);

        /// <summary>
        /// Suspends a tenant: revokes all active WorkspaceSessions for the tenant.
        /// Child company and membership record statuses are NOT modified.
        /// </summary>
        Task SuspendTenantAsync(Guid tenantId, CancellationToken ct = default);

        /// <summary>
        /// Cancels a company: cascades UserMemberships scoped to this company → Removed,
        /// their UserMembershipRoles → Removed, and active WorkspaceSessions → Revoked.
        /// </summary>
        Task CancelCompanyAsync(Guid companyId, Guid actorUserId, CancellationToken ct = default);

        /// <summary>
        /// Suspends a company: revokes all active WorkspaceSessions for the company.
        /// Membership and MembershipRole record statuses are NOT modified.
        /// </summary>
        Task SuspendCompanyAsync(Guid companyId, CancellationToken ct = default);

        /// <summary>
        /// Handles side-effects of revoking a UserApplicationRole assignment:
        /// revokes all active WorkspaceSessions for the user scoped to the same
        /// TenantId + CompanyId as the revoked assignment.
        /// </summary>
        Task RevokeRoleAssignmentAsync(Guid userId, Guid tenantId, Guid companyId, CancellationToken ct = default);
    }
}
