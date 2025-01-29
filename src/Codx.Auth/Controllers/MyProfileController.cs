using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class MyProfileController : Controller
    {
        protected readonly UserDbContext _userdbcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;

        public MyProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UserDbContext userdbcontext, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userdbcontext = userdbcontext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var user = _userManager.Users.FirstOrDefault(o => o.UserName == User.Identity.Name);

            var viewModel = _mapper.Map<MyProfileViewModel>(user);
            viewModel.FirstName = User.GetUserFirstName();
            viewModel.LastName = User.GetUserLastName();

            if(user.DefaultCompanyId.HasValue)
            {
                var userDefaultCompany = _userdbcontext.Companies.Include(c => c.Tenant).FirstOrDefault(c => c.Id == user.DefaultCompanyId);
                viewModel.CompanyName = userDefaultCompany?.Name;
                viewModel.TenantName = userDefaultCompany?.Tenant?.Name;
            }           

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetMyClaimsTableData(string search, string sort, string order, int offset, int limit)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserClaims.Where(o => o.UserId == userId);

            var data = query.OrderBy(o => o.Id).Skip(offset).Take(limit).ToList();
            var viewModel = data.Select(claim => new UserClaimDetailsViewModel
            {
                Id = claim.Id,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            }).ToList();

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public JsonResult GetMyRolesTableData(string search, string sort, string order, int offset, int limit)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserRoles.Include(o => o.Role).Where(o => o.UserId == userId);

            var data = query.Skip(offset).Take(limit).ToList();
            var viewModel = data.Select(userrole => new UserRoleDetailsViewModel
            {
                RoleId = userrole.RoleId,
                Role = userrole.Role.Name,
            }).ToList();

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public JsonResult GetMyTenantsTableData(string search, string sort, string order, int offset, int limit)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.TenantManagers.Include(o => o.Tenant).Where(o => o.UserId == userId);

            var data = query.OrderBy(o => o.Tenant.Name).Skip(offset).Take(limit).ToList();
            var viewModel = data.Select(tenant => new TenantManagerDetailsViewModel
            {
                UserId = tenant.UserId,
                TenantId = tenant.TenantId,
                Tenant = tenant.Tenant.Name,
            }).ToList();

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public JsonResult GetMyCompaniesTableData(string search, string sort, string order, int offset, int limit)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserCompanies.Include(o => o.Company).ThenInclude(c => c.Tenant).Where(o => o.UserId == userId);

            var data = query.OrderBy(o => o.Company.Name).Skip(offset).Take(limit).ToList();
            var viewModel = data.Select(userCompany => new UserCompanyDetailsViewModel
            {
                UserId = userCompany.UserId,
                CompanyId = userCompany.CompanyId,
                CompanyName = userCompany.Company.Name,
                TenantId = userCompany.Company.TenantId,
                TenantName = userCompany.Company.Tenant.Name
            }).ToList();

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public IActionResult ManageTenant(Guid id)
        {
            var userId = User.GetUserId();
            var tenantManager = _userdbcontext.TenantManagers.FirstOrDefault(o => o.UserId == userId && o.TenantId == id);

            if (tenantManager == null)
            {
                return RedirectToAction("Index");
            }

            var record = _userdbcontext.Tenants.FirstOrDefault(o => o.Id == id);

            var viewModel = _mapper.Map<TenantDetailsViewModel>(record);

            return View(viewModel);
        }

        public async Task<IActionResult> ManageTenantEdit(Guid id)
        {
            var userId = User.GetUserId();
            var tenantManager = _userdbcontext.TenantManagers.FirstOrDefault(o => o.UserId == userId && o.TenantId == id);

            if (tenantManager == null)
            {
                return RedirectToAction("Index");
            }

            var record = _userdbcontext.Tenants.FirstOrDefault(o => o.Id == id);

            var viewModel = _mapper.Map<TenantEditViewModel>(record);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTenantEdit(TenantEditViewModel viewModel)
        {
            var isRecordFound = await _userdbcontext.Tenants.AsNoTracking().AnyAsync(u => u.Id == viewModel.Id);

            if (ModelState.IsValid && isRecordFound)
            {
                var userId = User.GetUserId();

                var record = _mapper.Map<Tenant>(viewModel);
                record.UpdatedAt = DateTime.Now;
                record.UpdatedBy = userId;

                _userdbcontext.Tenants.Update(record);
                var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(ManageTenant), new { id = record.Id });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);
        }
        
        public IActionResult GetManageTenantCompaniesTableData(Guid tenantid, string search, string sort, string order, int offset, int limit)
        {
            var query = _userdbcontext.Companies.Where(o => !o.IsDeleted && o.TenantId == tenantid);
            var data = query.OrderBy(o => o.Name).Skip(offset).Take(limit).ToList();

            var viewModel = _mapper.Map<List<CompanyDetailsViewModel>>(data);

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }

        public IActionResult ManageTenantCompanyDetails(Guid id)
        {
            var record = _userdbcontext.Companies.FirstOrDefault(o => o.Id == id);

            var viewModel = _mapper.Map<CompanyDetailsViewModel>(record);

            return View(viewModel);
        }

        public async Task<IActionResult> ManageTenantCompanyAdd(Guid tenantid)
        {
            var tenant = await _userdbcontext.Tenants.FirstOrDefaultAsync(o => o.Id == tenantid);

            var viewModel = new CompanyAddViewModel
            {
                TenantId = tenant.Id,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTenantCompanyAdd(CompanyAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId();

                var record = _mapper.Map<Company>(viewModel);
                record.IsActive = true;
                record.IsDeleted = false;
                record.CreatedBy = userId;
                record.CreatedAt = DateTime.Now;

                await _userdbcontext.Companies.AddAsync(record).ConfigureAwait(false);
                var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(ManageTenantCompanyDetails), new { id = record.Id });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);

        }

        public async Task<IActionResult> ManageTenantCompanyEdit(Guid id)
        {
            var record = _userdbcontext.Companies.FirstOrDefault(o => o.Id == id);

            var viewModel = _mapper.Map<CompanyEditViewModel>(record);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTenantCompanyEdit(CompanyEditViewModel viewModel)
        {
            var isRecordFound = await _userdbcontext.Companies.AsNoTracking().AnyAsync(u => u.Id == viewModel.Id);

            if (ModelState.IsValid && isRecordFound)
            {
                var userId = User.GetUserId();

                var record = _mapper.Map<Company>(viewModel);
                record.UpdatedAt = DateTime.Now;
                record.UpdatedBy = userId;

                _userdbcontext.Companies.Update(record);
                var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(ManageTenantCompanyDetails), new { id = record.Id });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageTenantCompanyDelete(Guid id)
        {
            var record = _userdbcontext.Companies.FirstOrDefault(o => o.Id == id);

            var viewModel = _mapper.Map<CompanyEditViewModel>(record);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTenantCompanyDelete(CompanyEditViewModel viewModel)
        {
            var isRecordFound = _userdbcontext.Companies.Any(o => o.Id == viewModel.Id);
            if (ModelState.IsValid && isRecordFound)
            {
                var record = _userdbcontext.Companies.FirstOrDefault(o => o.Id == viewModel.Id);
                record.IsDeleted = true;
                record.IsActive = false;
                record.UpdatedAt = DateTime.Now;
                record.UpdatedBy = User.GetUserId();

                _userdbcontext.Companies.Update(record);

                var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);
                if (result > 0)
                {
                    return RedirectToAction(nameof(ManageTenant), new { id = viewModel.TenantId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetManageTenantManagersTableData(Guid tenantid, string search, string sort, string order, int offset, int limit)
        {
            var query = _userdbcontext.TenantManagers.Include(o => o.Manager).Where(o => o.TenantId == tenantid);

            var data = query.OrderBy(o => o.Manager.UserName).Skip(offset).Take(limit).ToList();
            var viewModel = _mapper.Map<List<TenantManagerDetailsViewModel>>(data);

            return Json(new
            {
                total = query.Count(),
                rows = viewModel
            });
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("ChangePasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
    }
}
