using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codx.Auth.Authorization
{
    public class WorkspaceAdministratorRequirement : IAuthorizationRequirement { }

    public class WorkspaceAdministratorHandler
        : AuthorizationHandler<WorkspaceAdministratorRequirement>
    {
        private static readonly HashSet<string> QualifyingRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "TENANT_OWNER", "TENANT_ADMIN", "TENANT_MANAGER",
            "COMPANY_ADMIN", "COMPANY_MANAGER"
        };

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            WorkspaceAdministratorRequirement requirement)
        {
            var tenantId  = context.User.FindFirstValue("tenant_id");
            var companyId = context.User.FindFirstValue("company_id");

            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(companyId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var workspaceRoles = context.User.FindAll("workspace_role")
                                              .Select(c => c.Value);

            if (workspaceRoles.Any(r => QualifyingRoles.Contains(r)))
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}
