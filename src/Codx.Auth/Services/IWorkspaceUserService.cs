using Codx.Auth.Models.WorkspaceUsers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public interface IWorkspaceUserService
    {
        /// <summary>
        /// Returns a paginated list of workspace users for the given tenant, company, and
        /// client. The application_id is resolved internally from <c>ClientProperties</c>
        /// using the caller's <paramref name="clientId"/> claim.
        /// Returns <c>null</c> when no application_id is mapped to <paramref name="clientId"/>
        /// in ClientProperties — the controller should treat this as 403 Forbidden.
        /// </summary>
        Task<WorkspaceUsersResponse?> GetWorkspaceUsersAsync(
            Guid   tenantId,
            Guid   companyId,
            string clientId,
            int    page,
            int    pageSize,
            string? email,
            CancellationToken cancellationToken = default);
    }
}
