using System;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class EpidemiologicalAnalyticsReportRequest
{
    public DateTime? EpidemiologyDate { get; set; }
    public string? OutbreakDisease { get; set; }
    public string? OutbreakLocation { get; set; }
    public DateTime? OutbreakStartDate { get; set; }
    public DateTime? OutbreakEndDate { get; set; }
    public string? OutbreakStatus { get; set; }
}
