using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.Common
{
    /// <summary>
    /// Paged result wrapper for API endpoints that return paginated data
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    public class PagedResult<T> : ApiResult<IEnumerable<T>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public PagedResult(IEnumerable<T> data, int page, int pageSize, int totalRecords) 
            : base(data)
        {
            Page = page;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public static PagedResult<T> Create(
            IEnumerable<T> data, 
            int page, 
            int pageSize, 
            int totalRecords, 
            string message = null)
        {
            var result = new PagedResult<T>(data, page, pageSize, totalRecords)
            {
                Message = message
            };
            return result;
        }
    }
}
