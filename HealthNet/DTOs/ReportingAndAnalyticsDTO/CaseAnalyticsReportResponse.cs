using System;
using HealthNet.Utility;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class CaseAnalyticsReportResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int TotalCases { get; set; }
    public int Activecases { get; set; }
    public int InActiveCases { get; set; }
    public double ResolvedCasesPercentage { get; set; }
    public List<CaseResponse>? Data { get; set; }
}
