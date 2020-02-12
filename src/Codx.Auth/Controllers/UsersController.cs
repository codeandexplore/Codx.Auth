using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Auth.Controllers
{
    public class UsersController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // List all Users
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            var viewModel = users.Select(user => new UserDetailsViewModel { 
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            }).ToList();
                       
            return View(viewModel);
        }


        // Add User
        [HttpGet]
        public IActionResult Add()
        {
            return View();    
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddViewModel model)
        {
            var record = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(record, model.Password).ConfigureAwait(false);

            if (result.Succeeded)
            { 
                // Success
            }

            return View();
        }


        // Edit User

        // Delete User

    }
}