namespace HealthNet.DTOs.Pages
{
    public class PagedResponseDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T>? Items { get; set; }
    }
}
