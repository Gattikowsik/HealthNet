using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class ComplianceMetricsReportResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public double CompleteDocPercentage { get; set; }
    public int TotalCompliances { get; set; }
    public int CompliantRecords { get; set; }
    public int NonCompliantRecords { get; set; }
    public int PartiallyCompliantRecords { get; set; }
    public int PendingReviewRecords { get; set; }
    public List<ComplianceRecord>? Data { get; set; }
}