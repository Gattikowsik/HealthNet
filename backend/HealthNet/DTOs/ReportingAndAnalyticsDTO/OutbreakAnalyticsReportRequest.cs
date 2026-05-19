using System;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class OutbreakAnalyticsReportRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? Region { get; set; }
}
