using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PaginatedResult<T>
    {
        public PaginationMetaData Metadata { get; set; } = default!;
        public List<T> Items { get; set; } = [];
    };

    public class PaginationMetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    };

    public class PaginationHelper
    {
        // create paginated results 
        public static async Task<PaginatedResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            // calculate the number of records
            var count = await query.CountAsync();

            var items = await query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            // return paginated result
            return new PaginatedResult<T>
            {
                Metadata = new PaginationMetaData
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((count / (double)pageSize)),
                    PageSize = pageSize,
                    TotalCount = count


                },
                Items = items
            };
        }
    }
}
