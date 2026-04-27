using System;

namespace HealthNet.DTOs.LabReportDTO;

public class LabReportResponse
{
    public int ReportId { get; set; }
    public int TestId { get; set; }
    public string FileURI { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Status { get; set; }                // false = Not Verified
}
