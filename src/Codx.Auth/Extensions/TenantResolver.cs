using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Duende.IdentityServer.Validation;
using System;
using System.Linq;

namespace Codx.Auth.Extensions
{
    public interface ITenantResolver
    {
        Tenant ResolveTenant(ApplicationUser user);
        Company ResolveCompany(ApplicationUser user);
    }

    public class TenantResolver : ITenantResolver
    {
        private readonly UserDbContext _context;

        public TenantResolver(UserDbContext context)
        {
            _context = context;
        }

        public Tenant ResolveTenant(ApplicationUser user)
        {
            var company = ResolveCompany(user);
            if (company != null)
            {
                return _context.Tenants.FirstOrDefault(t => t.Id == company.TenantId);
            }
            return null;
        }

        public Company ResolveCompany(ApplicationUser user)
        {
            var company = _context.Companies.FirstOrDefault(c => c.Id == user.DefaultCompanyId);
            if (company != null)
            {
                return company;
            }
            return null;
        }
    }
}
