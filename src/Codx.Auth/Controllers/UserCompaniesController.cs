using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class UserCompaniesController : Controller
    {
        protected readonly UserDbContext _userdbcontext;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;
        public UserCompaniesController(UserDbContext userdbcontext, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userdbcontext = userdbcontext;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public JsonResult GetUserCompaniesTableData(Guid userid, string search, string sort, string order, int offset, int limit)
        {
            var query = _userdbcontext.UserCompanies.Include(o => o.Company).ThenInclude(c => c.Tenant).Where(o => o.UserId == userid);

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