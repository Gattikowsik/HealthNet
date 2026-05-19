using System.Linq;
using System.Threading.Tasks;
using HealthNet.DTOs.Pages;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Services.PaginationService
{
    public class PaginationService : IPaginationService
    {
        public async Task<PagedResponseDto<T>> PaginateAsync<T>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponseDto<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                Items = items
            };
        }
    }
}