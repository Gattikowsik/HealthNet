using System;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class ComplianceMetricsReportRequest
{
    public DateTime? DateFilter { get; set; }
    public string? TypeFilter { get; set; }
    public string? ResultFilter { get; set; }
}