using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.Models.Common;
using Codx.Auth.Models.DTOs;
using Codx.Auth.Services.Interfaces;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/my-profile")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class MyProfileApiController : ControllerBase
    {
        protected readonly UserDbContext _userdbcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;
        protected readonly IFilterService _filterService;
        private readonly ILogger<MyProfileApiController> _logger;

        public MyProfileApiController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            UserDbContext userdbcontext, 
            IMapper mapper,
            IFilterService filterService,
            ILogger<MyProfileApiController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userdbcontext = userdbcontext;
            _mapper = mapper;
            _filterService = filterService;
            _logger = logger;
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetMyClaimsTableData(
            [FromQuery] string search = null, 
            [FromQuery] string sort = null, 
            [FromQuery] string order = "asc", 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var query = _userdbcontext.UserClaims.Where(o => o.UserId == userId);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);
                
                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.ApplySearch(filter.SearchTerm, nameof(ApplicationUserClaim.ClaimType), nameof(ApplicationUserClaim.ClaimValue));
                }

                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(claim => new UserClaimDetailsViewModel
                {
                    Id = claim.Id,
                    UserId = userId,
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue,
                });

                // Get paged response
                var pagedResponse = await _filterService.CreatePagedResult(viewModelQuery, filter);
                
                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResult<object>.Fail($"Error retrieving claims: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }



        [HttpGet("roles")]
        public async Task<IActionResult> GetMyRolesTableData(
            [FromQuery] string search = null, 
            [FromQuery] string sort = null, 
            [FromQuery] string order = "asc", 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var query = _userdbcontext.UserRoles
                    .Include(o => o.Role)
                    .Where(o => o.UserId == userId);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);
                
                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    // Search in role name through the related Role entity
                    query = query.Where(ur => ur.Role.Name.Contains(filter.SearchTerm));
                }

                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(userrole => new UserRoleDetailsViewModel
                {
                    RoleId = userrole.RoleId,
                    Role = userrole.Role.Name,
                });

                // Get paged response
                var pagedResponse = await _filterService.CreatePagedResult(viewModelQuery, filter);
                
                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResult<object>.Fail($"Error retrieving roles: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("tenants/managed")]
        public async Task<IActionResult> GetMyManagedTenantsTableData(
            [FromQuery] string search = null, 
            [FromQuery] string sort = null, 
            [FromQuery] string order = "asc", 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var query = _userdbcontext.TenantManagers
                    .Include(o => o.Tenant)
                    .Where(o => o.UserId == userId);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);
                
                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    // Search in tenant name, email, and description
                    query = query.Where(tm => 
                        tm.Tenant.Name.Contains(filter.SearchTerm) || 
                        tm.Tenant.Email.Contains(filter.SearchTerm) || 
                        tm.Tenant.Description.Contains(filter.SearchTerm));
                }

                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(managedTenant => new TenantViewDto(managedTenant.Tenant));

                // Get paged result
                var pagedResult = await _filterService.CreatePagedResult(viewModelQuery, filter);
                
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenants");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResult<object>.Fail($"Error retrieving managed tenants: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("tenants/managed/{id}")]
        public async Task<ActionResult> GetMyManagedTenantDetails(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .Include(tm => tm.Tenant)
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == id);

                if (tenantManager == null || tenantManager.Tenant == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }

                var tenantDetails = new TenantViewDto(tenantManager.Tenant);
                return Ok(ApiResult<object>.Success(tenantDetails, "Managed tenant details retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenant details");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving managed tenant details: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetMyCompaniesTableData(
            [FromQuery] string search = null, 
            [FromQuery] string sort = null, 
            [FromQuery] string order = "asc", 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var query = _userdbcontext.UserCompanies
                    .Include(o => o.Company)
                    .ThenInclude(c => c.Tenant)
                    .Where(o => o.UserId == userId);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);
                
                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    // Search in company name, tenant name, company email, company description
                    query = query.Where(uc => 
                        uc.Company.Name.Contains(filter.SearchTerm) || 
                        uc.Company.Tenant.Name.Contains(filter.SearchTerm) ||
                        uc.Company.Email.Contains(filter.SearchTerm) ||
                        uc.Company.Description.Contains(filter.SearchTerm));
                }

                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(userCompany => new CompanyViewDto(userCompany.Company));

                // Get paged result
                var pagedResult = await _filterService.CreatePagedResult(viewModelQuery, filter);
                
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResult<object>.Fail($"Error retrieving companies: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("companies/{id}")]
        public async Task<IActionResult> GetCompanyDetails(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userCompany = await _userdbcontext.UserCompanies
                    .Include(uc => uc.Company)
                    .ThenInclude(c => c.Tenant)
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == id);
                if (userCompany == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }
                var companyDetails = new CompanyViewDto(userCompany.Company);
                return Ok(ApiResult<object>.Success(companyDetails, "Company details retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company details");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving company details: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("switch-company")]
        public async Task<IActionResult> SwitchCompany([FromBody] SwitchCompanyModel company)
        {
            if (string.IsNullOrEmpty(company?.CompanyId))
            {
                return BadRequest(ApiResult<object>.Fail("Company ID is required"));
            }
            
            if (!Guid.TryParse(company.CompanyId, out var companyIdGuid))
            {
                return BadRequest(ApiResult<object>.Fail("Invalid company ID format"));
            }

            try
            {
                var userId = User.GetUserId();
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return NotFound(ApiResult<object>.Fail("User not found", StatusCodes.Status404NotFound));
                }

                var userCompany = await _userdbcontext.UserCompanies
                    .Include(uc => uc.Company)
                    .ThenInclude(c => c.Tenant)
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyIdGuid);

                if (userCompany == null)
                {
                    return BadRequest(ApiResult<object>.Fail("User is not associated with the specified company"));
                }

                user.DefaultCompanyId = companyIdGuid;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return StatusCode(
                        StatusCodes.Status500InternalServerError, 
                        ApiResult<object>.Fail(errors, StatusCodes.Status500InternalServerError));
                }

                // Sign the user in again to update the claims in the session
                await _signInManager.RefreshSignInAsync(user);

                return Ok(ApiResult<object>.Success(new { 
                    companyId = companyIdGuid,
                    companyName = userCompany.Company.Name,
                    tenantId = userCompany.Company.TenantId,
                    tenantName = userCompany.Company.Tenant.Name
                }, "Company switched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error switching company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResult<object>.Fail($"Error switching company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }
    }

    public class SwitchCompanyModel
    { 
        public string CompanyId { get; set; }
    }
}
