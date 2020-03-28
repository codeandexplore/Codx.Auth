using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels.UserClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class UserClaimsController : Controller
    {
        protected readonly UserDbContext _userdbcontext;
        protected readonly UserManager<ApplicationUser> _userManager;
        public UserClaimsController(UserDbContext userdbcontext, UserManager<ApplicationUser> userManager)
        {
            _userdbcontext = userdbcontext;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> UserClaims(string userid)
        {
            var user = await _userdbcontext.Users.Include(i => i.UserClaims).FirstOrDefaultAsync(u => u.Id.ToString() == userid);

            var viewmodel = new UserClaimsDetailsViewModel();

            viewmodel.UserId = user.Id;
            viewmodel.Username = user.UserName;
            viewmodel.Email = user.Email;

            foreach (var claim in user.UserClaims)
            {
                viewmodel.Claims.Add(new ClaimDetailsViewModel
                {
                    ClaimId = claim.Id,
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(string userid)
        {
            var user = await _userdbcontext.Users.Include(i => i.UserClaims).FirstOrDefaultAsync(u => u.Id.ToString() == userid);

            var viewmodel = new UserClaimAddViewModel();

            viewmodel.UserId = user.Id;
            viewmodel.Username = user.UserName;
            viewmodel.Email = user.Email;
                      
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserClaimAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userdbcontext.Users.Include(i => i.UserClaims).FirstOrDefaultAsync(u => u.Id == viewmodel.UserId);
                               
                var result = await _userManager.AddClaimAsync(user, new Claim(viewmodel.ClaimType, viewmodel.ClaimValue)).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(UserClaims), new { userid = viewmodel.UserId });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ToString());
                }
            }            

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int claimid, string userid)
        {
            var user = await _userdbcontext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userid);

            var claim = await _userdbcontext.UserClaims.FirstOrDefaultAsync(u => u.Id == claimid);

            var viewmodel = new UserClaimDeleteViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                ClaimId = claimid,
                ClaimType = claim.ClaimType,            
                ClaimValue = claim.ClaimValue,            
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserClaimDeleteViewModel viewmodel)
        {
            var user = await _userdbcontext.Users.FirstOrDefaultAsync(u => u.Id == viewmodel.UserId);

            var result = await _userManager.RemoveClaimAsync(user, new Claim(viewmodel.ClaimType, viewmodel.ClaimValue)).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserClaims), new { userid = viewmodel.UserId });
            }

            return View(viewmodel);
        }
    }
}