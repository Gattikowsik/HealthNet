using System;

namespace HealthNet.DTOs.LabTestDTO;

public class LaboratoryTestingRequest
{
    public int PatientId { get; set; }          // Patient to create test for
    public string Type { get; set; } = null!;   // Blood / Swab / X-Ray
    public int TechnicianId { get; set; }       // Lab technician assigned
}