using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
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
            if (user != null && user.DefaultCompanyId.HasValue)
            {
                var claims = new List<Claim>();

                // Add tenant and company claims
                Company company = null;
                if (user.DefaultCompanyId.HasValue)
                {
                    company = _tenantResolver.ResolveCompany(user);
                }
                else
                {
                    company = _tenantResolver.ResolveFirstUserCompany(user);
                }

                if (company != null)
                {
                    var tenant = company.Tenant;
                    claims.AddRange(new List<Claim>                    
                    {
                        new Claim("tenant_id", tenant.Id.ToString()),
                        new Claim("tenant_name", tenant.Name),
                        new Claim("company_id", company.Id.ToString()),
                        new Claim("company_name", company.Name)
                    });
                }

                // Add user claims
                var userClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims);

                // Add email claim if the scope includes email
                if (context.RequestedResources.ParsedScopes.Any(scope => scope.ParsedName == "email"))
                {
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                }

                // Add role claims
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

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
