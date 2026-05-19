using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class OutbreakAnalyticsReportResponse
{
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public int TotalOutbreaks { get; set; }
    public int ActiveOutbreaks { get; set; }
    public int ResolvedOutbreaks { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<Outbreak> Data { get; set; } = null!;
}
