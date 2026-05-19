using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class PatientAnalyticsReportResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int InActivePatients { get; set; }
    public List<Patient> Data { get; set; } = null!;
}
