using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels.RoleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {

        protected readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();

            var viewModel = roles.Select(role => new RoleDetailsViewModel
            {
                Id = role.Id,
                Name = role.Name,
            }).ToList();

            return View(viewModel);
        }

        // Add Role
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(RoleAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var record = new ApplicationRole { Name = viewModel.Name };

                var result = await _roleManager.CreateAsync(record).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ToString());
                }
            }

            return View(viewModel);
        }

        // Edit Role
        public async Task<IActionResult> Edit(string id)
        {
            var record = await _roleManager.FindByIdAsync(id);

            var viewModel = new RoleEditViewModel
            {
                Id = record.Id,
                Name = record.Name,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditViewModel viewmodel)
        {
            var record = await _roleManager.FindByIdAsync(viewmodel.Id.ToString());

            if (ModelState.IsValid)
            {
                record.Name = viewmodel.Name;

                var result = await _roleManager.UpdateAsync(record);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.ToString());
                }                
            }

            return View(viewmodel);
        }


        // Delete Role
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var record = await _roleManager.FindByIdAsync(id);

            var viewmodel = new RoleEditViewModel
            {
                Id = record.Id,
                Name = record.Name,
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RoleEditViewModel viewmodel)
        {
            var record = await _roleManager.FindByIdAsync(viewmodel.Id.ToString());

            var result = await _roleManager.DeleteAsync(record);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(viewmodel);
        }


    }
}