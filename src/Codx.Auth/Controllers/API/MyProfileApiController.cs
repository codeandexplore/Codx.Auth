using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/my-profile")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class MyProfileApiController : ControllerBase
    {
        protected readonly UserDbContext _userdbcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;

        public MyProfileApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UserDbContext userdbcontext, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userdbcontext = userdbcontext;
            _mapper = mapper;
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetMyClaimsTableData(string search, string sort, string order, int offset, int limit = 10)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserClaims.Where(o => o.UserId == userId);

            var data = await query.OrderBy(o => o.Id).Skip(offset).Take(limit).ToListAsync();
            var viewModel = data.Select(claim => new UserClaimDetailsViewModel
            {
                Id = claim.Id,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            }).ToList();

            return Ok(new
            {
                total = await query.CountAsync(),
                data = viewModel
            });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetMyRolesTableData(string search, string sort, string order, int offset, int limit = 10)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserRoles.Include(o => o.Role).Where(o => o.UserId == userId);

            var data = await query.Skip(offset).Take(limit).ToListAsync();
            var viewModel = data.Select(userrole => new UserRoleDetailsViewModel
            {
                RoleId = userrole.RoleId,
                Role = userrole.Role.Name,
            }).ToList();

            return Ok(new
            {
                total = await query.CountAsync(),
                data = viewModel
            });
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetMyCompaniesTableData(string search, string sort, string order, int offset, int limit=10)
        {
            var userId = User.GetUserId();
            var query = _userdbcontext.UserCompanies.Include(o => o.Company).ThenInclude(c => c.Tenant).Where(o => o.UserId == userId);

            var data = await query.OrderBy(o => o.Company.Name).Skip(offset).Take(limit).ToListAsync();
            var viewModel = data.Select(userCompany => new UserCompanyDetailsViewModel
            {
                UserId = userCompany.UserId,
                CompanyId = userCompany.CompanyId,
                CompanyName = userCompany.Company.Name,
                TenantId = userCompany.Company.TenantId,
                TenantName = userCompany.Company.Tenant.Name
            }).ToList();

            return Ok(new
            {
                total = await query.CountAsync(),
                data = viewModel
            });
        }

        [HttpPost("switch-company")]
        public async Task<IActionResult> SwitchCompany([FromBody] SwitchCompanyModel company)
        {
            if (!Guid.TryParse(company.CompanyId, out var companyIdGuid))
            {
                return BadRequest("Invalid company ID format.");
            }

            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userCompany = await _userdbcontext.UserCompanies
                .Include(uc => uc.Company)
                .ThenInclude(c => c.Tenant)
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyIdGuid);

            if (userCompany == null)
            {
                return BadRequest("User is not associated with the specified company.");
            }

            user.DefaultCompanyId = companyIdGuid;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user.");
            }
            
            // Sign the user in again to update the claims in the session
            await _signInManager.RefreshSignInAsync(user);

            return Ok("Company switched successfully.");
        }
    }

    public class SwitchCompanyModel
    { 
        public string CompanyId { get; set; }
    }
}
