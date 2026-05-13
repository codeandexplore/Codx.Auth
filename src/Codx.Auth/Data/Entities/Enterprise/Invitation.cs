using System;
using System.Collections.Generic;

namespace Codx.Auth.Data.Entities.Enterprise
{
    /// <summary>
    /// Represents a workspace invitation. InviteTokenHash stores SHA-256(rawToken).
    /// The raw token is never persisted — it is sent to the invitee via email only.
    /// </summary>
    public class Invitation
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        /// <summary>SHA-256 hash of the raw invite token. Never store the raw token.</summary>
        public string InviteTokenHash { get; set; }
        public string Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid InvitedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // ── Workspace-invite fields (spec 003-03) ──────────────────────────
        /// <summary>FK → EnterpriseApplications(Id). Null for legacy role-based invitations.</summary>
        public string ApplicationId { get; set; }
        /// <summary>JSON-serialised string[] of application role names to assign on acceptance.
        /// e.g. ["User"] or ["Manager","User"]. Null for legacy invitations.</summary>
        public string ApplicationRoles { get; set; }
        /// <summary>Workspace membership role to assign. Always "MEMBER" in this flow. Null for legacy.</summary>
        public string MembershipRole { get; set; }
        /// <summary>Server-resolved redirect URI after successful acceptance.
        /// Sourced from ClientProperties[Key="invitation_redirect_uri"] — never from caller.</summary>
        public string RedirectUri { get; set; }

        public ICollection<InvitationRole> InvitationRoles { get; set; }
    }
}
