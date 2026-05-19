using System;

namespace HealthNet.DTOs.PatientDto;

public class DeactivatePatientDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
