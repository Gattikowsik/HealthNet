using HealthNet.DTOs.Pages;

namespace HealthNet.Services.PaginationService
{
    public interface IPaginationService
    {
        Task<PagedResponseDto<T>> PaginateAsync<T>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize);
    }
}
