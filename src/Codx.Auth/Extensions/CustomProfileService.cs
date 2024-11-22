using Codx.Auth.Data.Entities.AspNet;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codx.Auth.Extensions
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantResolver _tenantResolver;

        public CustomProfileService(UserManager<ApplicationUser> userManager, ITenantResolver tenantResolver)
        {
            _userManager = userManager;
            _tenantResolver = tenantResolver;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user != null)
            {
                var tenant = _tenantResolver.ResolveTenant(context.RequestedResources);
                var company = _tenantResolver.ResolveCompany(context.RequestedResources);

                var claims = new List<Claim>
                {   
                    new Claim("tenant_id", tenant.Id.ToString()),
                    new Claim("tenant_name", tenant.Name),
                    new Claim("company_id", company.Id.ToString()),
                    new Claim("company_name", company.Name)
                };

                context.IssuedClaims.AddRange(claims);
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
