namespace Codx.Auth.Models.Common
{
    /// <summary>
    /// Filter class to standardize pagination parameters across API endpoints
    /// </summary>
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }

        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = DefaultPageSize;
            SortDirection = "asc";
        }

        public PaginationFilter(int pageNumber, int pageSize, string searchTerm = null, string sortColumn = null, string sortDirection = "asc")
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > MaxPageSize ? MaxPageSize : (pageSize < 1 ? DefaultPageSize : pageSize);
            SearchTerm = searchTerm?.Trim();
            SortColumn = sortColumn;
            SortDirection = !string.IsNullOrEmpty(sortDirection) && 
                            (sortDirection.ToLower() == "asc" || sortDirection.ToLower() == "desc") ? 
                            sortDirection.ToLower() : "asc";
        }
    }
}
