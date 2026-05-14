using System;

namespace HealthNet.DTOs.MedicalRecordDto;

public class MedicalRecordGetDto
{
    public int RecordId{get;set;}
    public DateOnly Date { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
}
