using AutoMapper;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // List all Users
        public IActionResult Index()
        {           
            return View();
        }

        [HttpGet]
        public JsonResult GetUsersTableData(string search, string sort, string order, int offset, int limit)
        {
            var roles = _userManager.Users;
            var userroles = roles.OrderBy(o => o.Id).Skip(offset).Take(limit).ToList();
            var viewModel = userroles.Select(user => new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            }).ToList();

            return Json(new
            {
                total = roles.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var record = _userManager.Users.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<UserDetailsViewModel>(record);

            return View(viewmodel);
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
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("Username", "Username is required.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Password did not match.");
                return View(model);
            }

            var existingEmailUser = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (existingEmailUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            var record = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(record, model.Password).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ToString());
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
                UserName = record.UserName,
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
                UserName = record.UserName,
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