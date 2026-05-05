using System;
using HealthNet.Utility;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class EpidemiologicalAnalyticsReportResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int TotalEpidemiologies { get; set; }
    public int ActiveEpidemiologies { get; set; }
    public int ActiveOutbreaks { get; set; }
    public int InActiveOutbreaks { get; set; }
    public List<EpidemiologyResponse>? EpidemiologyResponses { get; set; }
}
