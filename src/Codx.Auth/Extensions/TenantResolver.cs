using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.Enterprise;
using Duende.IdentityServer.Validation;
using System;
using System.Linq;

namespace Codx.Auth.Extensions
{
    public interface ITenantResolver
    {
        Tenant ResolveTenant(ResourceValidationResult requestedResources);
        Company ResolveCompany(ResourceValidationResult requestedResources);
    }

    public class TenantResolver : ITenantResolver
    {
        private readonly UserDbContext _context;

        public TenantResolver(UserDbContext context)
        {
            _context = context;
        }

        public Tenant ResolveTenant(ResourceValidationResult requestedResources)
        {
            var tenantScope = requestedResources.Resources.ApiScopes.FirstOrDefault(s => s.Name.StartsWith("tenant_id"));
            if (tenantScope != null && Guid.TryParse(tenantScope.Name.Split(':').Last(), out var tenantGuid))
            {
                return _context.Tenants.FirstOrDefault(t => t.Id == tenantGuid);
            }
            return null;
        }

        public Company ResolveCompany(ResourceValidationResult requestedResources)
        {
            var companyScope = requestedResources.Resources.ApiScopes.FirstOrDefault(s => s.Name.StartsWith("company_id"));
            if (companyScope != null && Guid.TryParse(companyScope.Name.Split(':').Last(), out var companyGuid))
            {
                return _context.Companies.FirstOrDefault(c => c.Id == companyGuid);
            }
            return null;
        }
    }
}
