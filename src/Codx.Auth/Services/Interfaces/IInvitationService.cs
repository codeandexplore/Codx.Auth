using Codx.Auth.Data.Entities.Enterprise;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Services.Interfaces
{
    public class InvitationValidationResult
    {
        public bool IsValid { get; set; }
        /// <summary>not_found | pending | accepted | revoked | expired</summary>
        public string ErrorCode { get; set; }
        public Guid InvitationId { get; set; }
        public string Email { get; set; }
        public Guid TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public IReadOnlyList<int> RoleIds { get; set; }
    }

    public interface IInvitationService
    {
        Task<(bool success, string error)> CreateInvitationAsync(
            Guid tenantId, Guid? companyId, IReadOnlyList<int> roleIds, string email, Guid invitedByUserId);

        /// <summary>
        /// Creates a workspace invitation (spec 003-03). Stores ApplicationId, serialised
        /// ApplicationRoles, MembershipRole, and RedirectUri on the Invitation record.
        /// Dispatches the invitation email using the company-branded template if configured.
        /// Returns the new Invitation.Id on success.
        /// </summary>
        Task<(bool success, Guid invitationId, string error)> CreateWorkspaceInvitationAsync(
            string email,
            Guid tenantId,
            Guid companyId,
            string applicationId,
            IReadOnlyList<string> applicationRoles,
            string membershipRole,
            string redirectUri,
            Guid createdByUserId,
            CancellationToken cancellationToken = default);

        Task<InvitationValidationResult> ValidateInviteTokenAsync(string rawToken);

        Task<Invitation> GetByIdAsync(Guid invitationId);

        Task<(bool success, string error)> AcceptInvitationAsync(Guid invitationId, Guid userId);

        Task<(bool success, string error)> RevokeInvitationAsync(Guid invitationId, Guid actorUserId);
    }
}
