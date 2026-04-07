using System;

namespace HealthNet.DTOs.LabTestDTO;

public class LabTestRequest
{
    public int PatientId { get; set; }          // Patient to create test for
    public string Type { get; set; } = null!;   // Blood / Swab / X-Ray
    public DateTime Date { get; set; }          // Scheduled date
    public int TechnicianId { get; set; }       // Lab technician assigned
}