using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public interface IAccountService
    {
        Task<(RegisterResponse result, ApplicationUser user)> RegisterAsync(RegisterRequest request);
        Task<(RegisterResponse result, ApplicationUser user)> RegisterExternalUserAsync(string email, string firstName, string lastName, string provider, string providerUserId);
    }

    public class AccountService : IAccountService
    {
        private readonly UserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(UserDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<(RegisterResponse result, ApplicationUser user)> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Enable 2FA for internal users (users created with passwords)
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                
                await InitializeNewUserAsync(user, request.Email, request.FirstName, request.LastName);
                return (new RegisterResponse { Success = true }, user);
            }

            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return (new RegisterResponse { Success = false, Errors = errors }, null);
        }

        public async Task<(RegisterResponse result, ApplicationUser user)> RegisterExternalUserAsync(string email, string firstName, string lastName, string provider, string providerUserId)
        {
            // Create user with a unique username since email might not be available from external provider
            var userName = !string.IsNullOrEmpty(email) ? email : Guid.NewGuid().ToString();
            var user = new ApplicationUser 
            { 
                UserName = userName, 
                Email = email 
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Do NOT enable 2FA for external users - they use their provider's 2FA
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                
                // Add external login
                var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                if (!addLoginResult.Succeeded)
                {
                    // If adding external login fails, clean up the user
                    await _userManager.DeleteAsync(user);
                    var loginErrors = addLoginResult.Errors.Select(e => e.Description).ToList();
                    return (new RegisterResponse { Success = false, Errors = loginErrors }, null);
                }

                // Initialize user with default tenant, company, roles, and claims
                await InitializeNewUserAsync(user, email, firstName, lastName);
                
                return (new RegisterResponse { Success = true }, user);
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return (new RegisterResponse { Success = false, Errors = errors }, null);
        }

        private async Task InitializeNewUserAsync(ApplicationUser user, string email, string firstName, string lastName)
        {
            // Add default role
            await _userManager.AddToRoleAsync(user, "User");
            
            // Add claims
            var claims = new List<Claim>();
            
            if (!string.IsNullOrEmpty(firstName))
                claims.Add(new Claim("given_name", firstName));
            
            if (!string.IsNullOrEmpty(lastName))
                claims.Add(new Claim("family_name", lastName));
            
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new Claim("email", email));
                claims.Add(new Claim("name", EmailAddressHelper.GetEmailUsername(email)));
            }
            else
            {
                // Fallback name if email is not available
                var displayName = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) 
                    ? $"{firstName} {lastName}" 
                    : (!string.IsNullOrEmpty(firstName) ? firstName : "External User");
                claims.Add(new Claim("name", displayName));
            }

            if (claims.Any())
            {
                await _userManager.AddClaimsAsync(user, claims);
            }

            // Create default tenant and company
            var tenantEmail = !string.IsNullOrEmpty(email) ? email : $"user-{user.Id}@external.com";
            var defaultTenant = new Tenant
            {
                Name = "Default",
                Description = "Default Tenant",
                Email = tenantEmail,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Id,
                IsDeleted = false,
                IsActive = true,
                TenantManagers = new List<TenantManager>
                {
                    new TenantManager
                    {
                        UserId = user.Id,
                    }
                },
                Companies = new List<Company>
                {
                    new Company
                    {
                        Name = "Default",
                        Description = "Default Company",
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Id,
                        IsDeleted = false,
                        IsActive = true,
                        UserCompanies = new List<UserCompany>
                        {
                            new UserCompany
                            {
                                UserId = user.Id,
                            }
                        }
                    }
                }
            };

            await _context.Tenants.AddAsync(defaultTenant);
            user.DefaultCompanyId = defaultTenant.Companies.FirstOrDefault().Id;
            await _context.SaveChangesAsync();
        }
    }
}
