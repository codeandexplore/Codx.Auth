using Codx.Auth.Models.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Services.Interfaces
{
    /// <summary>
    /// Service for handling filtering, sorting, searching, and pagination functionality
    /// </summary>
    public interface IFilterService
    {
        /// <summary>
        /// Creates a paged result from a queryable collection using the provided filter
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="query">The queryable collection to paginate</param>
        /// <param name="filter">The pagination filter parameters</param>
        /// <returns>A paged response containing the requested data</returns>
        Task<PagedResult<T>> CreatePagedResult<T>(
            IQueryable<T> query,
            PaginationFilter filter);
                        
        /// <summary>
        /// Creates a pagination filter from page and pageSize parameters
        /// </summary>
        /// <param name="page">The page number (1-based)</param>
        /// <param name="pageSize">The page size (number of items per page)</param>
        /// <param name="search">The search term to filter results</param>
        /// <param name="sort">The column to sort by</param>
        /// <param name="order">The sort direction (asc/desc)</param>
        /// <returns>A configured pagination filter</returns>
        PaginationFilter CreateFilter(
            int page, 
            int pageSize, 
            string search = null, 
            string sort = null, 
            string order = "asc");
    }
}
