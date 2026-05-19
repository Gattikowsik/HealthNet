using System;

namespace HealthNet.DTOs.PatientDto;

public class UpdatePatientResponseDto
{ 
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
