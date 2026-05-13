using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.WorkspaceUsers
{
    /// <summary>
    /// Carries all caller context resolved from JWT claims before the service call.
    /// The controller extracts claims and builds this object — the service never reads HttpContext.
    /// </summary>
    public record WorkspaceAddCallerContext
    {
        public required Guid TenantId { get; init; }
        public required Guid CompanyId { get; init; }

        /// <summary>
        /// The OAuth client_id claim (string). The service uses this to resolve
        /// application_id from ClientProperties — consistent with GetWorkspaceUsersAsync.
        /// </summary>
        public required string ClientId { get; init; }

        /// <summary>All workspace_role claim values for the calling user.</summary>
        public required IReadOnlyList<string> CallerWorkspaceRoles { get; init; }

        /// <summary>Caller's user ID from the sub claim. Stored as AssignedByUserId on created records.</summary>
        public required Guid CallerId { get; init; }
    }
}
