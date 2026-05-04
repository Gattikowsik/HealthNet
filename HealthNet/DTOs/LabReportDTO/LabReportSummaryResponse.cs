using System;

namespace HealthNet.DTOs.LabReportDTO;

public class LabReportSummaryResponse
{
    public int ReportId { get; set; }
    public string FileURI { get; set; } = null!;  // URL to access the lab report file
    public DateTime Date { get; set; }
    public bool Status { get; set; }                // false = Not Verified, true = Verified
}