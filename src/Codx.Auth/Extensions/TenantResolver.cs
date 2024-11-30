using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Duende.IdentityServer.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Codx.Auth.Extensions
{
    public interface ITenantResolver
    {
        Tenant ResolveTenant(ApplicationUser user);
        Tenant ResolveTenant(Guid tenantId);
        Company ResolveCompany(ApplicationUser user);
        Company ResolveCompany(Guid companyId);
        Company ResolveFirstUserCompany(ApplicationUser user);
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

        public Tenant ResolveTenant(Guid tenantId)
        {
            var tenant = _context.Tenants.FirstOrDefault(c => c.Id == tenantId);
            if (tenant != null)
            {
                return tenant;
            }
            return null;
        }

        public Company ResolveCompany(ApplicationUser user)
        {
            if (!user.DefaultCompanyId.HasValue)
            {
                return null;
            }
            var company = ResolveCompany(user.DefaultCompanyId.Value);
            if (company != null)
            {
                return company;
            }
            return null;
        }

        public Company ResolveCompany(Guid companyId)
        {
            var company = _context.Companies.Include(c => c.Tenant).FirstOrDefault(c => c.Id == companyId);
            if (company != null)
            {
                return company;
            }
            return null;
        }

        public Company ResolveFirstUserCompany(ApplicationUser user)
        {
            var userCompany = _context.UserCompanies.Include(uc => uc.Company).ThenInclude(c => c.Tenant).FirstOrDefault(uc => uc.UserId == user.Id);
            if (userCompany != null)
            {
                return userCompany.Company;
            }
            return null;
        }
    }
}
