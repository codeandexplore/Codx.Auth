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
            if (string.IsNullOrEmpty(model.Username))
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return View(model);
            }

            var record = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(record, model.Password).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // Edit User
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var record = await _userManager.FindByIdAsync(id);

            var viewmodel = new UserEditViewModel
            {
                Id = record.Id,
                Username = record.UserName,
                Email = record.Email
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel viewmodel)
        {
            var record = await _userManager.FindByIdAsync(viewmodel.Id.ToString());

            if (ModelState.IsValid)
            {
                record.Email = viewmodel.Email;

                var result = await _userManager.UpdateAsync(record);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(viewmodel);
        }

        // Delete User
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var record = await _userManager.FindByIdAsync(id);

            var viewmodel = new UserEditViewModel
            {
                Id = record.Id,
                Username = record.UserName,
                Email = record.Email
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserEditViewModel viewmodel)
        {
            var record = await _userManager.FindByIdAsync(viewmodel.Id.ToString());
            
            var result = await _userManager.DeleteAsync(record);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
        
            return View(viewmodel);
        }



    }
}