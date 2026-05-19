using System;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class CaseAnalyticsReportRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}
