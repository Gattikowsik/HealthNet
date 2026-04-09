using HealthNetDb.Entities;

namespace HealthNet.DTOs.CitizenSymptomReportingDTO
{
    public class SymptomReportResponseDto
    {
        public int ReportId { get; set; }
        public string? SymptomsJson { get; set; }
        public DateTime Date { get; set; }
        public SymptomStatus Status { get; set; }

    }
}
