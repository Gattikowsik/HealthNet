using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.ReportingAndAnalyticsDTO;

public class PatientAnalyticsReportResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int TotalPatients { get; set; }
    public int RegisteredPatients { get; set; }
    public int UnderTreatmentPatients { get; set; }
    public int RecoveredPatients { get; set; }
    public int DischargedPatients { get; set; }
    public List<Patient> Data { get; set; } = null!;
}
