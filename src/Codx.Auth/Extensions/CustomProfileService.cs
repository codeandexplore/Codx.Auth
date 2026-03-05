using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System;
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
            if (user == null) return;

            var claims = new List<Claim>();

            // Determine if this is for an ID token or Access token
            bool isIdToken = context.Caller == "ClaimsProviderIdentityToken";
            bool isAccessToken = context.Caller == "ClaimsProviderAccessToken";

            // For both ID Token and Access Token: Add tenant and company claims
            Company company = user.DefaultCompanyId.HasValue
                ? _tenantResolver.ResolveCompany(user)
                : _tenantResolver.ResolveFirstUserCompany(user);

            if (company != null)
            {
                var tenant = company.Tenant;

                claims.AddRange(new[]
                {
                    new Claim("tenant_id", tenant.Id.ToString()),
                    new Claim("tenant_name", tenant.Name),
                    new Claim("company_id", company.Id.ToString()),
                    new Claim("company_name", company.Name)
                });
            }

            if (isIdToken)
            {
                // ID Token: Include profile/identity claims
                
                // Subject claim is required
                var subClaim = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub");
                if (subClaim != null)
                {
                    claims.Add(subClaim);
                }

                // User Claims (custom claims from the database) - given_name/family_name now sourced from DB fields
                var userClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims.Where(c => c.Type != "given_name" && c.Type != "family_name"));

                // given_name from DB field
                if (!string.IsNullOrWhiteSpace(user.GivenName))
                {
                    claims.Add(new Claim("given_name", user.GivenName));
                }

                // family_name from DB field
                if (!string.IsNullOrWhiteSpace(user.FamilyName))
                {
                    claims.Add(new Claim("family_name", user.FamilyName));
                }

                // Get the claim types that are already added from user claims
                var existingClaimTypes = claims.Select(c => c.Type).ToHashSet();
                
                // Email - only add if not already present
                if (!string.IsNullOrWhiteSpace(user.Email) && !existingClaimTypes.Contains("email"))
                {
                    claims.Add(new Claim("email", user.Email));
                }
                
                // Email verified - only add if email exists
                if (!string.IsNullOrWhiteSpace(user.Email) && !existingClaimTypes.Contains("email_verified"))
                {
                    claims.Add(new Claim("email_verified", "true"));
                }

                // Name - computed from DB-backed name fields, fall back to username
                if (!existingClaimTypes.Contains("name"))
                {
                    var fullName = BuildDisplayName(user);
                    claims.Add(new Claim("name", string.IsNullOrWhiteSpace(fullName) ? user.UserName : fullName));
                }

                // Preferred username - only add if not already present
                if (!string.IsNullOrWhiteSpace(user.UserName) && !existingClaimTypes.Contains("preferred_username"))
                {
                    claims.Add(new Claim("preferred_username", user.UserName));
                }
            }
            else if (isAccessToken)
            {
                // Access Token: Include authorization claims (roles, permissions)
                
                // Roles
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim("role", role)));

                // Optionally include email for API identification purposes
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    claims.Add(new Claim("email", user.Email));
                }
            }
            else
            {
                // For other contexts (UserInfo endpoint, etc.), include all relevant claims
                
                // User Claims - given_name/family_name now sourced from DB fields
                var userClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims.Where(c => c.Type != "given_name" && c.Type != "family_name"));

                // given_name from DB field
                if (!string.IsNullOrWhiteSpace(user.GivenName))
                {
                    claims.Add(new Claim("given_name", user.GivenName));
                }

                // family_name from DB field
                if (!string.IsNullOrWhiteSpace(user.FamilyName))
                {
                    claims.Add(new Claim("family_name", user.FamilyName));
                }

                // Get the claim types that are already added
                var existingClaimTypes = claims.Select(c => c.Type).ToHashSet();
                
                // Email - only add if not already present
                if (!string.IsNullOrWhiteSpace(user.Email) && !existingClaimTypes.Contains("email"))
                {
                    claims.Add(new Claim("email", user.Email));
                }
                
                // Email verified - only add if email exists
                if (!string.IsNullOrWhiteSpace(user.Email) && !existingClaimTypes.Contains("email_verified"))
                {
                    claims.Add(new Claim("email_verified", "true"));
                }

                // Name - computed from DB-backed name fields, fall back to username
                if (!existingClaimTypes.Contains("name"))
                {
                    var fullName = BuildDisplayName(user);
                    claims.Add(new Claim("name", string.IsNullOrWhiteSpace(fullName) ? user.UserName : fullName));
                }

                // Preferred username - only add if not already present
                if (!string.IsNullOrWhiteSpace(user.UserName) && !existingClaimTypes.Contains("preferred_username"))
                {
                    claims.Add(new Claim("preferred_username", user.UserName));
                }

                // Roles
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim("role", role)));
            }

            // For ID tokens, include claims regardless of RequestedClaimTypes
            // since the profile scope might not have all custom claims configured
            if (isIdToken)
            {
                context.IssuedClaims.AddRange(claims);
            }
            else if (context.RequestedClaimTypes.Any())
            {
                // For access tokens and other contexts, filter by requested claim types if specified
                context.IssuedClaims.AddRange(claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)));
            }
            else
            {
                // If no specific claim types requested, include all
                context.IssuedClaims.AddRange(claims);
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }

        private static string BuildDisplayName(ApplicationUser user)
        {
            var parts = new[] { user.GivenName, user.MiddleName, user.FamilyName }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts).Trim();
        }
    }
}
