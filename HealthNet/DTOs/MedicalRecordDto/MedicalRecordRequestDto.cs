using System;
using System.ComponentModel.DataAnnotations;
using HealthNetDb.Enums;

namespace HealthNet.DTOs.MedicalRecordDto;

public class MedicalRecordRequestDto
{
    [Required]
    public string Diagnosis { get; set; } = null!;

    [Required]
    public string TreatmentPlan { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public PatientStatus Status { get; set; }
}
