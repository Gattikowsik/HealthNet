using System;
using System.ComponentModel.DataAnnotations;

namespace HealthNet.DTOs.LabTestDTO;

public class LaboratoryTestingUpdateRequest
{
    // Nullable — only update if provided
    public string? Type { get; set; }           // Blood / Swab / X-Ray

    [Range(1, int.MaxValue, ErrorMessage = "TechnicianId must be greater than 0.")]
    public int? TechnicianId { get; set; }      // Reassign technician — Doctor only
}
