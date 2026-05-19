using System;
using System.ComponentModel.DataAnnotations;

namespace HealthNet.DTOs.LabReportDTO;

public class LabReportRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TestId must be greater than 0.")]
    public int TestId { get; set; }         // Must exist in LabTest table

    [Required]
    public IFormFile File { get; set; } = null!; 
}