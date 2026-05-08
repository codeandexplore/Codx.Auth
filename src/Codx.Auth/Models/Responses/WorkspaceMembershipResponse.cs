using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.Responses
{
    public class WorkspaceMembershipResponse
    {
        public Guid MembershipId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string ContextType { get; set; }
        public Guid? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime JoinedAt { get; set; }
        public string ApplicationRole { get; set; }
        public List<string> WorkspaceRoles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
    }
}
