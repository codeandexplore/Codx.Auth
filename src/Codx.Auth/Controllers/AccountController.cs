// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.Services;
using Codx.Auth.Services.Interfaces;
using Codx.Auth.ViewModels;
using Codx.Auth.ViewModels.Account;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountService accountService,
            ITwoFactorService twoFactorService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _twoFactorService = twoFactorService;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registerRequest = new RegisterRequest
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                };
                
                var (result, user) = await _accountService.RegisterAsync(registerRequest);

                if (result.Success)
                {                    
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Handle returnUrl after registration
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        // Check for authorization context
                        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                return this.LoadingPage("Redirect", model.ReturnUrl);
                            }
                            return Redirect(model.ReturnUrl);
                        }

                        // Check if the returnUrl is local to prevent open redirect attacks
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                    }

                    return RedirectToAction("Index", "MyProfile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(model);
        }


        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);           

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            if(vm.ClientId == "tuos-ui")
            {
                return View("LoginTuosUI", vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return RedirectToAction("Index", "MyProfile");
                }
            }

            if (ModelState.IsValid)
            {
                // Attempt to sign in without lockout first to check credentials
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    
                    // Check if user has 2FA enabled using built-in ASP.NET Identity field
                    if (user.TwoFactorEnabled)
                    {
                        // Sign out the user temporarily and redirect to 2FA
                        await _signInManager.SignOutAsync();
                        
                        // Send 2FA code
                        var displayName = user.GetDisplayName();
                        var (success, code, message) = await _twoFactorService.SendVerificationCodeAsync(user.Id, user.Email, displayName);
                        
                        if (success)
                        {
                            // Store user info in temp data for 2FA verification
                            TempData["2FA_UserId"] = user.Id.ToString();
                            TempData["2FA_ReturnUrl"] = model.ReturnUrl;
                            TempData["2FA_RememberLogin"] = model.RememberLogin;
                            
                            return RedirectToAction("TwoFactor", new { email = user.Email });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Failed to send verification code. Please try again.");
                        }
                    }
                    else
                    {
                        // Complete login for users without 2FA enabled
                        await _signInManager.SignInAsync(user, model.RememberLogin);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                return this.LoadingPage("Redirect", model.ReturnUrl);
                            }
                            return Redirect(model.ReturnUrl);
                        }

                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return RedirectToAction("Index", "MyProfile");
                        }
                        else
                        {
                            throw new Exception("invalid return URL");
                        }
                    }
                }
                else
                {
                    // Now attempt with lockout for failed attempts
                    result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                    
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Account is locked out. Please try again later.");
                    }
                    else
                    {
                        await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                        ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                    }
                }
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Show Two-Factor Authentication page
        /// </summary>
        [HttpGet]
        public IActionResult TwoFactor(string email)
        {
            if (string.IsNullOrEmpty(email) || !TempData.ContainsKey("2FA_UserId"))
            {
                return RedirectToAction("Login");
            }

            var model = new TwoFactorViewModel
            {
                Email = email,
                ReturnUrl = TempData.Peek("2FA_ReturnUrl")?.ToString() ?? string.Empty,
                RememberLogin = TempData.Peek("2FA_RememberLogin") as bool? ?? false,
                Message = "We've sent a 6-digit verification code to your email address. Please enter it below to complete your login."
            };

            return View(model);
        }

        /// <summary>
        /// Handle Two-Factor Authentication verification
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model, string button)
        {
            if (button == "resend")
            {
                return await ResendTwoFactorCode(model);
            }

            if (!TempData.ContainsKey("2FA_UserId"))
            {
                return RedirectToAction("Login");
            }

            var userIdString = TempData.Peek("2FA_UserId")?.ToString();
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var isValidCode = await _twoFactorService.ValidateVerificationCodeAsync(userId, model.Code);
                
                if (isValidCode)
                {
                    // Code is valid, complete the login
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, model.RememberLogin);
                        
                        // Clear temp data
                        TempData.Remove("2FA_UserId");
                        var returnUrl = TempData["2FA_ReturnUrl"]?.ToString();
                        TempData.Remove("2FA_ReturnUrl");
                        TempData.Remove("2FA_RememberLogin");

                        // Check for authorization context
                        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                return this.LoadingPage("Redirect", returnUrl);
                            }
                            return Redirect(returnUrl);
                        }

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "MyProfile");
                    }
                }
                else
                {
                    ModelState.AddModelError("Code", "Invalid or expired verification code. Please try again.");
                    model.IsError = true;
                    model.Message = "The verification code you entered is invalid or has expired.";
                }
            }

            return View(model);
        }

        /// <summary>
        /// Resend Two-Factor Authentication code
        /// </summary>
        private async Task<IActionResult> ResendTwoFactorCode(TwoFactorViewModel model)
        {
            if (!TempData.ContainsKey("2FA_UserId"))
            {
                return RedirectToAction("Login");
            }

            var userIdString = TempData.Peek("2FA_UserId")?.ToString();
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var displayName = user.GetDisplayName();
                var (success, code, message) = await _twoFactorService.SendVerificationCodeAsync(user.Id, user.Email, displayName);
                
                if (success)
                {
                    model.Message = "A new verification code has been sent to your email address.";
                    model.IsError = false;
                }
                else
                {
                    model.Message = "Failed to resend verification code. Please try again.";
                    model.IsError = true;
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

            model.Code = string.Empty; // Clear the code field
            return View(model);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray(),
                ClientId = context?.Client.ClientId,
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

    }
}