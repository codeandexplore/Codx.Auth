using Codx.Auth.Extensions;
using Codx.Auth.Models.Common;
using Codx.Auth.Models.Responses;
using Codx.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/my-memberships")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class MembershipsApiController : ControllerBase
    {
        private readonly IMembershipQueryService _membershipQueryService;
        private readonly ILogger<MembershipsApiController> _logger;

        public MembershipsApiController(
            IMembershipQueryService membershipQueryService,
            ILogger<MembershipsApiController> logger)
        {
            _membershipQueryService = membershipQueryService;
            _logger = logger;
        }

        /// <summary>
        /// Returns all workspace memberships for the authenticated user.
        /// The IsActive flag is computed against the tenant_id / company_id claims in the token.
        /// The ApplicationRole is resolved for the application identified by the token's client_id.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(WorkspaceMembershipResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyMemberships(CancellationToken ct)
        {
            try
            {
                var userId = User.GetUserId();

                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
                var companyIdClaim = User.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value;
                var clientId = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value ?? string.Empty;

                Guid.TryParse(tenantIdClaim, out var activeTenantId);
                Guid? activeCompanyId = Guid.TryParse(companyIdClaim, out var parsedCompanyId)
                    ? parsedCompanyId
                    : (Guid?)null;

                var memberships = await _membershipQueryService.GetCompanyMembershipsAsync(
                    userId, activeTenantId, activeCompanyId, clientId, ct);

                return Ok(memberships);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving memberships");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResult<object>.Fail($"Error retrieving memberships: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }
    }
}
