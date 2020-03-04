using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels.RoleViewModels;
using Codx.Auth.ViewModels.UserRoleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var user = await _userdbcontext.Users.Include(i => i.UserRoles).FirstOrDefaultAsync(u => u.Id.ToString() == userid);
                       
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

        

    }
}