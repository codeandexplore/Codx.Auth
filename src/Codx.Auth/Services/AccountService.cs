
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public interface IAccountService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
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

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("given_name", request.FirstName));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("family_name", request.LastName));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", request.Email));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", EmailHelper.GetEmailUsername(request.Email)));

                var defaultTenant = new Tenant
                {
                    Name = "Default",
                    Description = "Default Tenant",
                    Email = request.Email,
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

                return new RegisterResponse { Success = true };
            }

            
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return new RegisterResponse { Success = false, Errors = errors };
        }
    }
}
