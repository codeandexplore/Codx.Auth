using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
