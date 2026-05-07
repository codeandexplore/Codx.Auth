using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Extensions;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/admin")]
    [Authorize(Policy = "PlatformAdmin")]
    [ApiController]
    public class LifecycleApiController : ControllerBase
    {
        private readonly UserDbContext _db;
        private readonly LifecycleTransitionGuard _guard;
        private readonly ILifecycleCascadeService _cascade;
        private readonly IAuditService _audit;

        public LifecycleApiController(
            UserDbContext db,
            LifecycleTransitionGuard guard,
            ILifecycleCascadeService cascade,
            IAuditService audit)
        {
            _db = db;
            _guard = guard;
            _cascade = cascade;
            _audit = audit;
        }

        public sealed class StatusTransitionRequest
        {
            [Required]
            public string Status { get; set; }
            public string Reason { get; set; }
        }

        private IActionResult InvalidTransition(string entityType, string currentStatus, string requestedStatus)
            => Conflict(new
            {
                type = "https://codx.auth/errors/invalid-lifecycle-transition",
                title = "Invalid lifecycle transition",
                status = 409,
                detail = $"{entityType} cannot transition from '{currentStatus}' to '{requestedStatus}'.",
                currentStatus,
                requestedStatus
            });

        // PATCH /api/v1/admin/tenants/{id}/status
        [HttpPatch("tenants/{id:guid}/status")]
        public async Task<IActionResult> PatchTenantStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (tenant == null) return NotFound();

            var guardResult = _guard.Validate("Tenant", tenant.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("Tenant", tenant.Status, request.Status);

            var oldStatus = tenant.Status;
            tenant.Status = request.Status;
            tenant.UpdatedAt = DateTime.UtcNow;
            tenant.UpdatedBy = actorId;

            if (request.Status == LifecycleStatus.Tenant.Cancelled)
                await _cascade.CancelTenantAsync(id, actorId, ct);
            else if (request.Status == LifecycleStatus.Tenant.Suspended)
                await _cascade.SuspendTenantAsync(id, ct);
            else
                await _db.SaveChangesAsync(ct);

            await _audit.LogAsync("TenantStatusChanged",
                actorUserId: actorId,
                tenantId: id,
                resourceType: "Tenant",
                resourceId: id.ToString(),
                details: $"Status: '{oldStatus}' → '{request.Status}'. {request.Reason}".TrimEnd(' ', '.'));

            return Ok(new { id = tenant.Id, name = tenant.Name, status = tenant.Status, updatedAt = tenant.UpdatedAt });
        }

        // PATCH /api/v1/admin/companies/{id}/status
        [HttpPatch("companies/{id:guid}/status")]
        public async Task<IActionResult> PatchCompanyStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (company == null) return NotFound();

            var guardResult = _guard.Validate("Company", company.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("Company", company.Status, request.Status);

            var oldStatus = company.Status;
            company.Status = request.Status;
            company.UpdatedAt = DateTime.UtcNow;
            company.UpdatedBy = actorId;

            if (request.Status == LifecycleStatus.Company.Cancelled)
                await _cascade.CancelCompanyAsync(id, actorId, ct);
            else if (request.Status == LifecycleStatus.Company.Suspended)
                await _cascade.SuspendCompanyAsync(id, ct);
            else
                await _db.SaveChangesAsync(ct);

            await _audit.LogAsync("CompanyStatusChanged",
                actorUserId: actorId,
                tenantId: company.TenantId,
                companyId: id,
                resourceType: "Company",
                resourceId: id.ToString(),
                details: $"Status: '{oldStatus}' → '{request.Status}'. {request.Reason}".TrimEnd(' ', '.'));

            return Ok(new { id = company.Id, name = company.Name, status = company.Status, updatedAt = company.UpdatedAt });
        }

        // PATCH /api/v1/admin/memberships/{id}/status
        [HttpPatch("memberships/{id:guid}/status")]
        public async Task<IActionResult> PatchMembershipStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var membership = await _db.UserMemberships.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (membership == null) return NotFound();

            var guardResult = _guard.Validate("Membership", membership.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("Membership", membership.Status, request.Status);

            membership.Status = request.Status;
            membership.StatusChangedAt = DateTime.UtcNow;
            membership.StatusChangedBy = actorId;

            await _db.SaveChangesAsync(ct);

            await _audit.LogAsync("MembershipStatusChanged",
                actorUserId: actorId,
                userId: membership.UserId,
                tenantId: membership.TenantId,
                companyId: membership.CompanyId,
                resourceType: "UserMembership",
                resourceId: id.ToString());

            return Ok(new { id = membership.Id, status = membership.Status, statusChangedAt = membership.StatusChangedAt });
        }

        // PATCH /api/v1/admin/membership-roles/{id}/status
        [HttpPatch("membership-roles/{id:guid}/status")]
        public async Task<IActionResult> PatchMembershipRoleStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var membershipRole = await _db.UserMembershipRoles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (membershipRole == null) return NotFound();

            var guardResult = _guard.Validate("MembershipRole", membershipRole.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("MembershipRole", membershipRole.Status, request.Status);

            membershipRole.Status = request.Status;
            membershipRole.StatusChangedAt = DateTime.UtcNow;
            membershipRole.StatusChangedBy = actorId;

            await _db.SaveChangesAsync(ct);

            return Ok(new { id = membershipRole.Id, status = membershipRole.Status, statusChangedAt = membershipRole.StatusChangedAt });
        }

        // PATCH /api/v1/admin/users/{id}/status
        [HttpPatch("users/{id:guid}/status")]
        public async Task<IActionResult> PatchUserStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null) return NotFound();

            var guardResult = _guard.Validate("User", user.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("User", user.Status, request.Status);

            user.Status = request.Status;
            user.StatusChangedAt = DateTime.UtcNow;
            user.StatusChangedBy = actorId;

            await _db.SaveChangesAsync(ct);

            await _audit.LogAsync("UserStatusChanged",
                actorUserId: actorId,
                userId: id,
                resourceType: "User",
                resourceId: id.ToString(),
                details: $"Status changed. {request.Reason}".TrimEnd());

            return Ok(new { id = user.Id, email = user.Email, status = user.Status, statusChangedAt = user.StatusChangedAt });
        }

        // PATCH /api/v1/admin/enterprise-applications/{id}/status
        [HttpPatch("enterprise-applications/{id}/status")]
        public async Task<IActionResult> PatchEnterpriseApplicationStatus(string id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var app = await _db.EnterpriseApplications.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (app == null) return NotFound();

            var guardResult = _guard.Validate("Application", app.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("EnterpriseApplication", app.Status, request.Status);

            app.Status = request.Status;
            app.IsActive = request.Status == LifecycleStatus.Application.Active;

            await _db.SaveChangesAsync(ct);

            return Ok(new { id = app.Id, displayName = app.DisplayName, status = app.Status });
        }

        // PATCH /api/v1/admin/enterprise-application-roles/{id}/status
        [HttpPatch("enterprise-application-roles/{id:guid}/status")]
        public async Task<IActionResult> PatchEnterpriseApplicationRoleStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var role = await _db.EnterpriseApplicationRoles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (role == null) return NotFound();

            var guardResult = _guard.Validate("AppRole", role.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("EnterpriseApplicationRole", role.Status, request.Status);

            role.Status = request.Status;
            role.IsActive = request.Status == LifecycleStatus.AppRole.Active;

            await _db.SaveChangesAsync(ct);

            return Ok(new { id = role.Id, applicationId = role.ApplicationId, name = role.Name, status = role.Status });
        }

        // PATCH /api/v1/admin/workspace-role-definitions/{id}/status
        [HttpPatch("workspace-role-definitions/{id:int}/status")]
        public async Task<IActionResult> PatchWorkspaceRoleDefinitionStatus(int id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var roleDef = await _db.WorkspaceRoleDefinitions.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (roleDef == null) return NotFound();

            var guardResult = _guard.Validate("RoleDefinition", roleDef.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("WorkspaceRoleDefinition", roleDef.Status, request.Status);

            roleDef.Status = request.Status;
            roleDef.IsActive = request.Status == LifecycleStatus.RoleDefinition.Active;

            await _db.SaveChangesAsync(ct);

            return Ok(new { id = roleDef.Id, code = roleDef.Code, displayName = roleDef.DisplayName, status = roleDef.Status });
        }

        // PATCH /api/v1/admin/role-assignments/{id}/status
        [HttpPatch("role-assignments/{id:guid}/status")]
        public async Task<IActionResult> PatchRoleAssignmentStatus(Guid id, [FromBody] StatusTransitionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var actorId = User.GetUserId();
            var assignment = await _db.UserApplicationRoles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (assignment == null) return NotFound();

            var guardResult = _guard.Validate("RoleAssignment", assignment.Status, request.Status);
            if (!guardResult.IsValid) return InvalidTransition("RoleAssignment", assignment.Status, request.Status);

            assignment.Status = request.Status;
            assignment.RevokedAt = DateTime.UtcNow;
            assignment.RevokedByUserId = actorId;

            // Cascade: revoke active workspace sessions for this user scoped to (TenantId, CompanyId)
            await _cascade.RevokeRoleAssignmentAsync(assignment.UserId, assignment.TenantId, assignment.CompanyId, ct);

            await _audit.LogAsync("RoleAssignmentRevoked",
                actorUserId: actorId,
                userId: assignment.UserId,
                tenantId: assignment.TenantId,
                companyId: assignment.CompanyId,
                resourceType: "UserApplicationRole",
                resourceId: id.ToString(),
                details: request.Reason);

            return Ok(new
            {
                id = assignment.Id,
                userId = assignment.UserId,
                applicationId = assignment.ApplicationId,
                roleId = assignment.RoleId,
                status = assignment.Status,
                revokedAt = assignment.RevokedAt,
                revokedByUserId = assignment.RevokedByUserId
            });
        }
    }
}
