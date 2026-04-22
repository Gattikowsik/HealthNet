using System;
using System.ComponentModel.DataAnnotations;
using HealthNetDb.Enums;
namespace HealthNet.DTOs.PatientDto;

public class RegisterPatientRequestDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public DateOnly DOB { get; set; }

    [Required]
    public string Gender { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string ContactInfo { get; set; } = null!;

    [Required]
    public PatientStatus Status { get; set; }
}