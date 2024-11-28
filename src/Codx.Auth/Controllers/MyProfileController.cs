using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class MyProfileController : Controller
    {
        protected readonly UserDbContext _userdbcontext;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;

        public MyProfileController(UserManager<ApplicationUser> userManager, UserDbContext userdbcontext, IMapper mapper)
        {
            _userManager = userManager;
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
    }
}
