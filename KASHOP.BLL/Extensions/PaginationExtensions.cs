using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KASHOP.DAL.Dto;
using System.Threading.Tasks;

namespace KASHOP.BLL.Extensions
{
    public static class PaginationExtensions
    {

        // for any list of data, we can use this extension method to paginate the data based on the page number and page size
        public static async Task<paginationResponse<T>> ToPaginateAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = query.Count();
            var data = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new paginationResponse<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
