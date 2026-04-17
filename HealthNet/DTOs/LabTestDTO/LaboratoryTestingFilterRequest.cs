using System;

namespace HealthNet.DTOs.LabTestDTO;

public class LaboratoryTestingFilterRequest
{
    public string? Type { get; set; }       // Blood / Swab / X-Ray
    public string? Status { get; set; }     // "Pending" or "Completed"
    public DateOnly? Date { get; set; }     // YYYY-MM-DD
}