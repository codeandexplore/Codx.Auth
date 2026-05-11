using Codx.Auth.Data.Contexts;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Models.WorkspaceUsers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    public class WorkspaceUserService : IWorkspaceUserService
    {
        private readonly UserDbContext _userDb;
        private readonly IdentityServerDbContext _isDb;

        public WorkspaceUserService(UserDbContext userDb, IdentityServerDbContext isDb)
        {
            _userDb = userDb;
            _isDb   = isDb;
        }

        public async Task<WorkspaceUsersResponse?> GetWorkspaceUsersAsync(
            Guid   tenantId,
            Guid   companyId,
            string clientId,
            int    page,
            int    pageSize,
            string? email,
            CancellationToken cancellationToken = default)
        {
            // 1. Resolve application_id from ClientProperties via client_id claim
            var applicationId = await _isDb.Clients
                .Where(c => c.ClientId == clientId)
                .SelectMany(c => c.Properties)
                .Where(p => p.Key == "application_id")
                .Select(p => p.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (applicationId == null)
                return null;

            // 2. Build the user query with all lifecycle filters
            var query =
                from uar in _userDb.UserApplicationRoles
                join u   in _userDb.Users                    on uar.UserId  equals u.Id
                join ear in _userDb.EnterpriseApplicationRoles on uar.RoleId equals ear.Id
                join t   in _userDb.Tenants                  on uar.TenantId equals t.Id
                join c   in _userDb.Companies                on uar.CompanyId equals c.Id
                where uar.TenantId      == tenantId
                   && uar.CompanyId     == companyId
                   && uar.ApplicationId == applicationId
                   && uar.Status        == LifecycleStatus.RoleAssignment.Active
                   && t.Status          == LifecycleStatus.Tenant.Active
                   && c.TenantId        == tenantId
                   && c.Status          == LifecycleStatus.Company.Active
                   && _userDb.UserMemberships.Any(um =>
                          um.UserId    == uar.UserId
                       && um.TenantId  == tenantId
                       && (um.CompanyId == null || um.CompanyId == companyId)
                       && um.Status    == LifecycleStatus.Membership.Active)
                select new
                {
                    UserId          = u.Id,
                    u.GivenName,
                    u.MiddleName,
                    u.FamilyName,
                    u.UserName,
                    u.Email,
                    u.NormalizedEmail,
                    ApplicationRole = ear.Name,
                };

            // 3. Apply optional email filter
            if (!string.IsNullOrEmpty(email))
            {
                var upper = email.ToUpperInvariant();
                query = query.Where(x => x.NormalizedEmail != null && x.NormalizedEmail.Contains(upper));
            }

            // 4. Count (for pagination metadata) then fetch the page
            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);

            var rows = await query
                .OrderBy(x => x.GivenName)
                .ThenBy(x => x.FamilyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var data = rows
                .Select(x =>
                {
                    var nameParts = new[] { x.GivenName, x.MiddleName, x.FamilyName }
                        .Where(p => !string.IsNullOrWhiteSpace(p));
                    var fullName = string.Join(" ", nameParts).Trim();
                    if (string.IsNullOrEmpty(fullName))
                        fullName = x.UserName ?? x.Email ?? string.Empty;

                    return new WorkspaceUserDto(
                        x.UserId,
                        fullName,
                        x.Email    ?? string.Empty,
                        x.ApplicationRole);
                })
                .ToList();

            var pagination = new PaginationMeta(
                Page:           page,
                PageSize:       pageSize,
                TotalCount:     totalCount,
                TotalPages:     totalPages,
                HasNextPage:    page < totalPages,
                HasPreviousPage: page > 1);

            return new WorkspaceUsersResponse(data, pagination);
        }
    }
}
