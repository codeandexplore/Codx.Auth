using Codx.Auth.Data.Entities.AspNet;
using System;

namespace Codx.Auth.Data.Entities.Enterprise
{
    public class TenantManager
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public ApplicationUser Manager { get; set; }
        public Tenant Tenant { get; set; }
    }
}
