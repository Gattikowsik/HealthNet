using System;

namespace HealthNet.DTOs.LabReportDTO;

public class LabTestWithReportsResponse
{
    public int TestId { get; set; }
    public int PatientId { get; set; }
    public string Type { get; set; } = null!;
    public DateTime Date { get; set; }
    public int TechnicianId { get; set; }
    public bool TestStatus { get; set; }           // false = Pending, true = Completed

    // Note: LabReportSummaryResponse contains a subset of LabReportResponse fields for summary purposes
    public IEnumerable<LabReportSummaryResponse> Reports { get; set; } = new List<LabReportSummaryResponse>();
}