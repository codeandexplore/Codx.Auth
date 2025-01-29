using Codx.Auth.Data.Entities.Enterprise;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Codx.Auth.Data.Entities.AspNet
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
        public virtual ICollection<UserCompany> UserCompanies { get; set; }
        public virtual ICollection<TenantManager> TenantManagers { get; set; }

        public Guid? DefaultCompanyId { get; set; }
    }
}
