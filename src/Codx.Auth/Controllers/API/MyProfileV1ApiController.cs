using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/my-profile")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class MyProfileV1ApiController : ControllerBase
    {
        protected readonly UserDbContext _userdbcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;
        protected readonly IFilterService _filterService;
        private readonly ILogger<MyProfileV1ApiController> _logger;

        public MyProfileV1ApiController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            UserDbContext userdbcontext, 
            IMapper mapper,
            IFilterService filterService,
            ILogger<MyProfileV1ApiController> logger)
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

        [HttpPut("tenants/managed/{id}")]
        public async Task<IActionResult> UpdateMyManagedTenantDetails(Guid id, [FromBody] TenantEditDto tenant)
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

                if (tenant.Id != id)
                {
                    return BadRequest(ApiResult<object>.Fail("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                // Update tenant properties
                tenantManager.Tenant.Name = tenant.Name;
                tenantManager.Tenant.Email = tenant.Email;
                tenantManager.Tenant.Phone = tenant.Phone;
                tenantManager.Tenant.Address = tenant.Address;
                tenantManager.Tenant.Logo = tenant.Logo;
                tenantManager.Tenant.Theme = tenant.Theme;
                tenantManager.Tenant.Description = tenant.Description;
                _userdbcontext.Tenants.Update(tenantManager.Tenant);
                var result = await _userdbcontext.SaveChangesAsync();
                if (result <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResult<object>.Fail("Failed to update managed tenant", StatusCodes.Status500InternalServerError));
                }
                return Ok(ApiResult<object>.Success(new TenantViewDto(tenantManager.Tenant), "Managed tenant updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating managed tenant details");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error updating managed tenant details: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("tenants/managed/{id}/companies")]
        public async Task<IActionResult> GetMyManagedTenantCompaniesTableData(Guid id,
            [FromQuery] string search = null,
            [FromQuery] string sort = null,
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers.FirstOrDefaultAsync(o => o.UserId == userId && o.TenantId == id);

                if(tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }

                var query = _userdbcontext.Companies
                    .Include(o => o.Tenant)
                    .Where(c => c.TenantId == tenantManager.TenantId && !c.IsDeleted);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);

                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    // Search in company name, email, description, and tenant name
                    query = query.Where(uc =>
                        uc.Name.Contains(filter.SearchTerm) ||
                        uc.Email.Contains(filter.SearchTerm) ||
                        uc.Description.Contains(filter.SearchTerm));
                }
                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(company => new CompanyViewDto(company));

                // Get paged result
                var pagedResult = await _filterService.CreatePagedResult(viewModelQuery, filter);

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenant companies");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving managed tenant companies: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("tenants/managed/{tenantid}/companies")]
        public async Task<IActionResult> AddMyManagedTenantCompany(Guid tenantid, [FromBody] CompanyAddDto company)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .Include(o => o.Tenant)
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }

                var existingCompany = await _userdbcontext.Companies
                    .FirstOrDefaultAsync(c => c.Name == company.Name && c.TenantId == tenantManager.TenantId);

                if (existingCompany != null)
                {
                    return Conflict(ApiResult<object>.Fail("Company with this name already exists", StatusCodes.Status409Conflict));
                }

                var newCompany = new Company
                {
                    Name = company.Name,
                    Email = company.Email,
                    Phone = company.Phone,
                    Address = company.Address,
                    Logo = company.Logo,
                    Theme = company.Theme,
                    Description = company.Description,
                    TenantId = tenantManager.TenantId                  ,
                };

                var newCompanyUser = new UserCompany { UserId = userId };
                newCompany.UserCompanies = new List<UserCompany> { newCompanyUser };

                await _userdbcontext.Companies.AddAsync(newCompany);
                var result = await _userdbcontext.SaveChangesAsync();
                if (result <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResult<object>.Fail("Failed to add company", StatusCodes.Status500InternalServerError));
                }
                newCompany.Tenant = tenantManager.Tenant;
                return CreatedAtAction(
                    nameof(GetMyManagedTenantCompanyDetails),
                    new { tenantid, companyid = newCompany.Id },
                    ApiResult<object>.Success(new CompanyViewDto(newCompany), "Managed tenant company added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding managed tenant company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error adding managed tenant company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPut("tenants/managed/{tenantid}/companies/{companyid}")]
        public async Task<IActionResult> UpdateMyManagedTenantCompany(Guid tenantid, Guid companyid, [FromBody] CompanyEditDto company)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }
                var existingCompany = await _userdbcontext.Companies
                    .Include(c => c.Tenant)
                    .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (existingCompany == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }

                if (existingCompany.Id != companyid)
                {
                    return BadRequest(ApiResult<object>.Fail("Unauthorized", StatusCodes.Status401Unauthorized));
                }
                        
                var duplicateCompany = await _userdbcontext.Companies
                    .FirstOrDefaultAsync(c => c.Name == company.Name && c.TenantId == tenantManager.TenantId && c.Id != companyid);
                if (duplicateCompany != null)
                {
                    return Conflict(ApiResult<object>.Fail("Company with this name already exists", StatusCodes.Status409Conflict));
                }


                existingCompany.Name = company.Name;
                existingCompany.Email = company.Email;
                existingCompany.Phone = company.Phone;
                existingCompany.Address = company.Address;
                existingCompany.Logo = company.Logo;
                existingCompany.Theme = company.Theme;
                existingCompany.Description = company.Description;
                _userdbcontext.Companies.Update(existingCompany);
                var result = await _userdbcontext.SaveChangesAsync();
                if (result <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResult<object>.Fail("Failed to update company", StatusCodes.Status500InternalServerError));
                }
                return Ok(ApiResult<object>.Success(new CompanyViewDto(existingCompany), "Managed tenant company updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating managed tenant company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error updating managed tenant company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpDelete("tenants/managed/{tenantid}/companies/{companyid}")]
        public async Task<IActionResult> DeleteMyManagedTenantCompany(Guid tenantid, Guid companyid)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }
                var company = await _userdbcontext.Companies
                    .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }
                _userdbcontext.Companies.Remove(company);
                var result = await _userdbcontext.SaveChangesAsync();
                if (result <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResult<object>.Fail("Failed to delete company", StatusCodes.Status500InternalServerError));
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting managed tenant company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error deleting managed tenant company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("tenants/managed/{tenantid}/companies/{companyid}")]
        public async Task<IActionResult> GetMyManagedTenantCompanyDetails(Guid tenantid, Guid companyid)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }
                var company = await _userdbcontext.Companies
                    .Include(c => c.Tenant)
                    .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }
                var companyDetails = new CompanyViewDto(company);
                return Ok(ApiResult<object>.Success(companyDetails, "Managed tenant company details retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenant company details");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving managed tenant company details: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("tenants/managed/{tenantid}/companies/{companyid}/users")]
        public async Task<IActionResult> GetMyManagedTenantCompanyUsersTableData(Guid tenantid, Guid companyid,
            [FromQuery] string search = null,
            [FromQuery] string sort = null,
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }

                var company = await _userdbcontext.Companies
                   .Include(c => c.Tenant)
                   .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }

                var query = _userdbcontext.UserCompanies
                    .Include(uc => uc.Company)
                    .Include(uc => uc.User)
                    .Where(uc => uc.CompanyId == company.Id && uc.Company.TenantId == tenantManager.TenantId);

                // Create pagination filter from page and pageSize parameters
                var filter = _filterService.CreateFilter(page, pageSize, search, sort, order);
                // Apply search if provided
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    // Search in user name, email, and company name
                    query = query.Where(uc =>
                        uc.User.UserName.Contains(filter.SearchTerm) ||
                        uc.User.Email.Contains(filter.SearchTerm) ||
                        uc.Company.Name.Contains(filter.SearchTerm));
                }
                // Map to view model before pagination to ensure proper sorting
                var viewModelQuery = query.Select(userCompany => new UserViewDto(userCompany.User));
                // Get paged result
                var pagedResult = await _filterService.CreatePagedResult(viewModelQuery, filter);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenant company users");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving managed tenant company users: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("tenants/managed/{tenantid}/companies/{companyid}/users")]
        public async Task<IActionResult> AddMyManagedTenantCompanyUser(Guid tenantid, Guid companyid, [FromBody] CompanyUserAddDto companyUser)
        {
            try
            {
                var userId = User.GetUserId(); 
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }
                var company = await _userdbcontext.Companies
                   .Include(c => c.Tenant)
                   .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }

                var findByEmail = await _userManager.FindByEmailAsync(companyUser.EmailAddress);
                if (findByEmail == null)
                {
                    return UnprocessableEntity(ApiResult<object>.Fail("User with this email does not exist.", StatusCodes.Status422UnprocessableEntity));
                }

                // Check if user already exists
                var existingCompanyUser = await _userdbcontext.UserCompanies.FirstOrDefaultAsync(o => o.User.Email == findByEmail.Email && o.CompanyId == company.Id);
                if (existingCompanyUser != null)
                {
                    return Conflict(ApiResult<object>.Fail("User with this email already exists", StatusCodes.Status409Conflict));
                }

                // Create new company user
                var record = new UserCompany
                {
                    UserId = findByEmail.Id,
                    CompanyId = company.Id,
                    Company = company
                };

                await _userdbcontext.UserCompanies.AddAsync(record).ConfigureAwait(false);

                if (!record.User.DefaultCompanyId.HasValue)
                {
                    record.User.DefaultCompanyId = record.CompanyId;
                    _userdbcontext.Users.Update(record.User);
                }

                var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);

                if (result <= 0)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ApiResult<object>.Fail("Failed to add user to the company", StatusCodes.Status500InternalServerError));
                }

                return CreatedAtAction(
                    nameof(GetMyManagedTenantCompanyUserDetails),
                    new { tenantid, companyid, userid = findByEmail.Id },
                    ApiResult<object>.Success(null, "User added to managed tenant company successfully")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user to managed tenant company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error adding user to managed tenant company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
           
        }


        [HttpDelete("tenants/managed/{tenantid}/companies/{companyid}/users/{userid}")]
        public async Task<IActionResult> DeleteMyManagedTenantCompanyUser(Guid tenantid, Guid companyid, Guid userid)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }
                var company = await _userdbcontext.Companies
                   .Include(c => c.Tenant)
                   .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }

                var record = await _userdbcontext.UserCompanies.Include(uc => uc.User).FirstOrDefaultAsync(o => o.CompanyId == company.Id && o.UserId == userid);
                if (record != null)
                {
                    if(record.User.DefaultCompanyId == record.CompanyId)
                    {                       
                        record.User.DefaultCompanyId = null;
                        _userdbcontext.Users.Update(record.User);
                    }

                    _userdbcontext.UserCompanies.Remove(record);

                    var result = await _userdbcontext.SaveChangesAsync().ConfigureAwait(false);

                    if (result <= 0)
                    {
                        return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            ApiResult<object>.Fail("Failed to remove user from the company", StatusCodes.Status500InternalServerError));
                    }
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from managed tenant company");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error removing user from managed tenant company: {ex.Message}", StatusCodes.Status500InternalServerError));
            }

        }

        [HttpGet("tenants/managed/{tenantid}/companies/{companyid}/users/{userid}")]
        public async Task<IActionResult> GetMyManagedTenantCompanyUserDetails(Guid tenantid, Guid companyid, Guid userid)
        {
            try
            {
                var userId = User.GetUserId();
                var tenantManager = await _userdbcontext.TenantManagers
                    .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TenantId == tenantid);
                if (tenantManager == null)
                {
                    return NotFound(ApiResult<object>.Fail("Managed tenant not found", StatusCodes.Status404NotFound));
                }

                var company = await _userdbcontext.Companies
                   .Include(c => c.Tenant)
                   .FirstOrDefaultAsync(c => c.Id == companyid && c.TenantId == tenantManager.TenantId);
                if (company == null)
                {
                    return NotFound(ApiResult<object>.Fail("Company not found", StatusCodes.Status404NotFound));
                }

                var userCompany = await _userdbcontext.UserCompanies
                    .Include(uc => uc.Company)
                    .Include(uc => uc.User)
                    .FirstOrDefaultAsync(uc => uc.UserId == userid && uc.CompanyId == company.Id && uc.Company.TenantId == tenantManager.TenantId);
                if(userCompany == null)
                {
                    return NotFound(ApiResult<object>.Fail("User not found in the specified company", StatusCodes.Status404NotFound));
                }
             
                var userDetails = new UserViewDto(userCompany.User);
                return Ok(ApiResult<object>.Success(userDetails, "Managed tenant company user details retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving managed tenant company user details");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving managed tenant company user details: {ex.Message}", StatusCodes.Status500InternalServerError));
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
                    .Where(o => o.UserId == userId && !o.Company.IsDeleted);

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
