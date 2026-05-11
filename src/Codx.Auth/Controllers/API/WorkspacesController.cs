using Codx.Auth.Services;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers.API
{
    [ApiController]
    [Route("api/v1/workspaces")]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    [Authorize(Policy = "WorkspaceAdministratorPolicy")]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceUserService _service;

        public WorkspacesController(IWorkspaceUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns a paginated list of users with an active application role in
        /// the caller's current workspace (tenant + company). Tenant, company,
        /// and application are resolved exclusively from the bearer token claims.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWorkspaceUsers(
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 20,
            [FromQuery] string? email    = null,
            CancellationToken   cancellationToken = default)
        {
            if (page < 1)
                return BadRequest("page must be >= 1");

            pageSize = Math.Clamp(pageSize, 1, 100);

            var tenantId  = Guid.Parse(User.FindFirstValue("tenant_id")!);
            var companyId = Guid.Parse(User.FindFirstValue("company_id")!);
            var clientId  = User.FindFirstValue("client_id")!;

            var result = await _service.GetWorkspaceUsersAsync(
                tenantId, companyId, clientId, page, pageSize, email, cancellationToken);

            if (result is null)
                return Forbid();

            return Ok(result);
        }
    }
}
