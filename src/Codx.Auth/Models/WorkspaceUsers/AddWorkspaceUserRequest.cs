namespace Codx.Auth.Models.WorkspaceUsers
{
    public record AddWorkspaceUserRequest
    {
        /// <summary>Required. Target user's email address.</summary>
        public required string Email { get; init; }

        /// <summary>
        /// Optional. One of "Admin" | "Manager" | "User".
        /// When omitted, all EnterpriseApplicationRole rows with IsDefault = true are used.
        /// </summary>
        public string? ApplicationRole { get; init; }
    }
}
