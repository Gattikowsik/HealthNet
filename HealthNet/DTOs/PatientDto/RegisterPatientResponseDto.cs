using System;

namespace HealthNet.DTOs.PatientDto;

public class RegisterPatientResponseDto
{
    public int PatientId { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
}