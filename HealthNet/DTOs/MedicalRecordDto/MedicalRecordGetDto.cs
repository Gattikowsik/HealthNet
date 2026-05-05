using System;

namespace HealthNet.DTOs.MedicalRecordDto;

public class MedicalRecordGetDto
{
    public DateOnly Date { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
}
