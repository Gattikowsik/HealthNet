using HealthNetDb.Entities;

namespace HealthNet.DTOs.CitizenSymptomReportingDTO
{
    public class SymptomReportResponseDto
    {
        public int ReportId { get; set; }
        public int? CitizenId { get; set; }
        public string? CitizenName { get; set; }
        public string? SymptomsJson { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
