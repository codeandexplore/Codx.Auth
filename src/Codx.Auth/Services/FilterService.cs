using Codx.Auth.Extensions;
using Codx.Auth.Models.Common;
using Codx.Auth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Services
{
    /// <summary>
    /// Implementation of filter service for handling filtering, pagination, sorting and searching
    /// </summary>
    public class FilterService : IFilterService
    {
        public async Task<PagedResult<T>> CreatePagedResult<T>(
            IQueryable<T> query,
            PaginationFilter filter)
        {
            // Calculate total before applying paging
            var totalRecords = await query.CountAsync();

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                query = query.ApplySort(filter.SortColumn, filter.SortDirection);
            }

            // Apply paging
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var data = await query
                .Skip(skip)
                .Take(filter.PageSize)
                .ToListAsync();

            // Create paged result
            return PagedResult<T>.Create(
                data,
                filter.PageNumber,
                filter.PageSize,
                totalRecords);
        }
           
        
        public PaginationFilter CreateFilter(
            int page, 
            int pageSize, 
            string search = null, 
            string sort = null, 
            string order = "asc")
        {
            // Directly use page and pageSize parameters
            return new PaginationFilter(
                page,
                pageSize,
                search,
                sort,
                order);
        }
    }
}
