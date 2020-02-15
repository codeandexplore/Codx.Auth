using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Auth.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                //Validation of user
                var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");

            // something went wrong, show form with error
            var vm = BuildLoginViewModelAsync(model);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private LoginViewModel BuildLoginViewModelAsync(string returnUrl)
        {
            return new LoginViewModel
            {
                AllowRememberLogin = true,
                EnableLocalLogin = true,
                ReturnUrl = returnUrl,
            };
        }

        private LoginViewModel BuildLoginViewModelAsync(LoginInputViewModel model)
        {
            var vm = BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

    }
}