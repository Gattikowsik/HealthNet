using System;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class PatientAnalyticsReportRequest
{
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? Gender { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
