using System;

namespace HealthNet.DTOs.PatientDto;

public class UpdatePatientDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly DOB { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}
