using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {

        protected readonly UserDbContext _userdbcontext;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly RoleManager<ApplicationRole> _roleManager;
        public UserRolesController(UserDbContext userdbcontext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userdbcontext = userdbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> UserRoles(string userid)
        {
            var user = await _userdbcontext.Users.Include(i => i.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id.ToString() == userid);
                       
            var viewmodel = new UserRolesDetailsViewModel();

            viewmodel.UserId = user.Id;
            viewmodel.Username = user.UserName;
            viewmodel.Email = user.Email;

            foreach (var role in user.UserRoles)
            {
                viewmodel.Roles.Add(new RoleDetailsViewModel { 
                    Id = role.RoleId,
                    Name = role.Role.Name,
                });
            }
            
            return View(viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Add(string userid)
        {
            var user = await _userdbcontext.Users.Include(i => i.UserRoles).FirstOrDefaultAsync(u => u.Id.ToString() == userid);

            var viewmodel = new UserRoleAddViewModel();

            viewmodel.UserId = user.Id;
            viewmodel.Username = user.UserName;
            viewmodel.Email = user.Email;

            var roles = await _userdbcontext.Roles.ToListAsync().ConfigureAwait(false);
                        
            foreach (var role in roles)
            {
                viewmodel.Roles.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name,
                });
            }

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserRoleAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userdbcontext.Users.Include(i => i.UserRoles).FirstOrDefaultAsync(u => u.Id == viewmodel.UserId);

                var role = await _userdbcontext.Roles.FirstOrDefaultAsync(r => r.Id == viewmodel.RoleId).ConfigureAwait(false);

                var result = await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(UserRoles), new { userid=viewmodel.UserId });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ToString());
                }
            }

            var roles = await _userdbcontext.Roles.ToListAsync().ConfigureAwait(false);

            foreach (var role in roles)
            {
                viewmodel.Roles.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string userid, string roleid)
        {
            var user = await _userdbcontext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userid);

            var role = await _userdbcontext.Roles.FirstOrDefaultAsync(u => u.Id.ToString() == roleid);

            var viewmodel = new UserRoleDeleteViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                RoleId = role.Id,
                Rolename = role.Name,
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserRoleDeleteViewModel viewmodel)
        {
            var user = await _userdbcontext.Users.FirstOrDefaultAsync(u => u.Id == viewmodel.UserId);

            var result = await _userManager.RemoveFromRoleAsync(user, viewmodel.Rolename).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserRoles), new { userid = viewmodel.UserId });
            }

            return View(viewmodel);
        }


    }
}