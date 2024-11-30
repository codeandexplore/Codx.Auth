using Codx.Auth.Data.Entities.AspNet;
using System;

namespace Codx.Auth.Data.Entities.Enterprise
{
    public class UserCompany
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
    }
}
