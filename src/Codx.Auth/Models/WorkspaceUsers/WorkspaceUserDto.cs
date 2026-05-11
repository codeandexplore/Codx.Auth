using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.WorkspaceUsers
{
    public record WorkspaceUserDto(
        Guid   UserId,
        string FullName,
        string Email,
        string ApplicationRole);

    public record PaginationMeta(
        int  Page,
        int  PageSize,
        int  TotalCount,
        int  TotalPages,
        bool HasNextPage,
        bool HasPreviousPage);

    public record WorkspaceUsersResponse(
        IReadOnlyList<WorkspaceUserDto> Data,
        PaginationMeta Pagination);
}
