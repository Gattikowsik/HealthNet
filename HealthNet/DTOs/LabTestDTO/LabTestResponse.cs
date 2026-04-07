using System;

namespace HealthNet.DTOs.LabTestDTO;

public class LabTestResponse
{
    public int TestId { get; set; }             // Created TestID
    public int PatientId { get; set; }
    public string Type { get; set; } = null!;   // Blood / Swab / X-Ray
    public DateTime Date { get; set; }
    public int TechnicianId { get; set; }
    public bool Status { get; set; }            // false = Pending by default
}
