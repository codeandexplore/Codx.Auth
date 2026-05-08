using Codx.Auth.Data.Contexts;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/tenants")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class TenantApiController : ControllerBase
    {
        private readonly UserDbContext _db;
        private readonly IMembershipQueryService _membershipQueryService;

        public TenantApiController(UserDbContext db, IMembershipQueryService membershipQueryService)
        {
            _db = db;
            _membershipQueryService = membershipQueryService;
        }

        /// <summary>
        /// Returns all active company-scoped workspace memberships for the authenticated user.
        /// Only includes memberships where the membership, company and tenant are all active.
        /// Tenant-scoped memberships (CompanyId = null) are excluded from the list but their
        /// workspace roles are merged into each matching company entry.
        /// Source of truth for workspace selection — called after initial login.
        /// </summary>
        [HttpGet("/api/v1/memberships")]
        public async Task<IActionResult> GetMemberships(CancellationToken ct)
        {
            var subClaim = User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(subClaim, out var userId))
                return Unauthorized();

            var memberships = await _membershipQueryService.GetCompanyMembershipsAsync(userId, ct: ct);

            // Project to the workspace-select response shape (roles as string[] for compatibility)
            var result = memberships.Select(m => new
            {
                membershipId = m.MembershipId,
                tenantId = m.TenantId,
                tenantName = m.TenantName,
                companyId = m.CompanyId,
                companyName = m.CompanyName,
                contextType = m.ContextType,
                joinedAt = m.JoinedAt,
                roles = m.WorkspaceRoles,
            });

            return Ok(result);
        }

        /// <summary>
        /// Returns the workspace context embedded in the current bearer token.
        /// The SPA calls this after a page refresh to restore workspace state without
        /// re-parsing JWT claims directly in JavaScript.
        ///
        /// Returns 204 when the token carries no workspace context (Phase-1 / initial
        /// login token) so the SPA knows to redirect to the workspace selection page.
        /// </summary>
        [HttpGet("/api/v1/workspaces/active")]
        public IActionResult GetActiveWorkspace()
        {
            var tenantIdStr = User.FindFirst("tenant_id")?.Value;
            var contextType = User.FindFirst("workspace_context_type")?.Value;

            // No workspace context in this token — caller should redirect to workspace selection.
            if (string.IsNullOrEmpty(tenantIdStr) || string.IsNullOrEmpty(contextType))
                return NoContent();

            if (!Guid.TryParse(tenantIdStr, out var tenantId))
                return BadRequest(new { error = "malformed_token", detail = "tenant_id claim is not a valid GUID" });

            Guid? companyId = null;
            var companyIdStr = User.FindFirst("company_id")?.Value;
            if (!string.IsNullOrEmpty(companyIdStr) && Guid.TryParse(companyIdStr, out var parsedCompanyId))
                companyId = parsedCompanyId;

            var membershipIdStr = User.FindFirst("membership_id")?.Value;
            Guid.TryParse(membershipIdStr, out var membershipId);

            var sessionIdStr = User.FindFirst("workspace_session_id")?.Value;
            Guid.TryParse(sessionIdStr, out var sessionId);

            var workspaceRoles = User.FindAll("workspace_role")
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                tenantId,
                companyId,
                contextType,
                membershipId,
                sessionId,
                roles = workspaceRoles
            });
        }
        /// <summary>
        /// Returns all users who have an active company-scoped membership for the given company.
        /// Used by the Application User Assignments tab in the admin UI to populate the user picker.
        /// </summary>
        [HttpGet("/api/v1/companies/{companyId}/members")]
        public async Task<IActionResult> GetCompanyMembers(Guid companyId, [FromQuery] Guid tenantId)
        {
            // Enforce caller's tenant_id claim matches requested tenantId (prevents cross-tenant enumeration)
            var callerTenantIdStr = User.FindFirst("tenant_id")?.Value;
            if (!Guid.TryParse(callerTenantIdStr, out var callerTenantId) || callerTenantId != tenantId)
                return Problem(detail: "Caller's tenant context does not match the requested tenantId.", statusCode: 403);

            // Validate company belongs to tenant
            var companyExists = await _db.Companies
                .AnyAsync(c => c.Id == companyId && c.TenantId == tenantId);
            if (!companyExists)
                return Problem(detail: "Company not found for the specified tenant.", statusCode: 404);

            var members = await _db.UserMemberships
                .Where(m =>
                    m.CompanyId == companyId &&
                    m.TenantId == tenantId &&
                    m.Status == LifecycleStatus.Membership.Active)
                .Include(m => m.User)
                .AsNoTracking()
                .Select(m => new
                {
                    userId = m.UserId,
                    email = m.User.Email,
                    displayName = m.User.UserName ?? m.User.Email
                })
                .Distinct()
                .OrderBy(m => m.email)
                .ToListAsync();

            return Ok(members);
        }
    }
}

