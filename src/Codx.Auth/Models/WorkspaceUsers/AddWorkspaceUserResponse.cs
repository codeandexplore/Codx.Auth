using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.WorkspaceUsers
{
    public record AddWorkspaceUserResponse
    {
        /// <summary>"added" or "invited"</summary>
        public required string Outcome { get; init; }

        /// <summary>Populated on "added" path.</summary>
        public Guid? UserId { get; init; }

        /// <summary>Populated on "invited" path.</summary>
        public Guid? InvitationId { get; init; }

        public required string Email { get; init; }

        /// <summary>All application roles assigned or queued. One or more values.</summary>
        public required IReadOnlyList<string> ApplicationRoles { get; init; }
    }
}
