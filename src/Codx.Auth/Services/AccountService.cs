using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Extensions;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;
using Codx.Auth.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public interface IAccountService
    {
        Task<(RegisterResponse result, ApplicationUser user)> RegisterAsync(RegisterRequest request);
        Task<(RegisterResponse result, ApplicationUser user)> RegisterExternalUserAsync(string email, string firstName, string lastName, string provider, string providerUserId);
        Task<(bool success, string message)> SendEmailVerificationAsync(ApplicationUser user, string callbackUrl);
    }

    public class AccountService : IAccountService
    {
        private readonly UserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        
        public AccountService(
            UserDbContext context, 
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
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
                
                // External users' emails are already verified by the provider
                if (!string.IsNullOrEmpty(email))
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, token);
                }
                
                return (new RegisterResponse { Success = true }, user);
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return (new RegisterResponse { Success = false, Errors = errors }, null);
        }

        public async Task<(bool success, string message)> SendEmailVerificationAsync(ApplicationUser user, string callbackUrl)
        {
            try
            {
                var displayName = user.GetDisplayName();
                var subject = "Confirm Your Email Address";
                var body = CreateEmailVerificationBody(displayName, callbackUrl);

                var emailMessage = new EmailMessage
                {
                    To = user.Email,
                    Subject = subject,
                    Body = body,
                    IsHtml = true
                };

                var result = await _emailService.SendEmailAsync(emailMessage);
                
                if (result.Success)
                {
                    return (true, "Verification email sent successfully");
                }
                else
                {
                    return (false, $"Failed to send verification email: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error sending verification email: {ex.Message}");
            }
        }

        private string CreateEmailVerificationBody(string displayName, string callbackUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Email Verification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #007bff;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f8f9fa;
            padding: 30px;
            border-radius: 0 0 5px 5px;
        }}
        .button {{
            display: inline-block;
            padding: 15px 30px;
            background-color: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
            font-weight: bold;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 5px;
            padding: 15px;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 12px;
            color: #6c757d;
        }}
        .link {{
            word-break: break-all;
            color: #007bff;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Welcome to Codx Auth!</h1>
    </div>
    <div class=""content"">
        <h2>Hello {displayName},</h2>
        <p>Thank you for registering with Codx Auth. To complete your registration, please verify your email address by clicking the button below:</p>
        
        <div style=""text-align: center;"">
            <a href=""{callbackUrl}"" class=""button"">Verify Email Address</a>
        </div>
        
        <div class=""warning"">
            <strong>Important:</strong>
            <ul>
                <li>This link will expire in 24 hours</li>
                <li>If you did not create this account, please ignore this email</li>
                <li>Do not share this link with anyone</li>
            </ul>
        </div>
        
        <p>If the button above doesn't work, copy and paste the following link into your browser:</p>
        <p class=""link"">{callbackUrl}</p>
        
        <p>Best regards,<br>Codx Auth Team</p>
    </div>
    <div class=""footer"">
        <p>This is an automated message. Please do not reply to this email.</p>
        <p>If you have any questions, please contact our support team.</p>
    </div>
</body>
</html>";
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
