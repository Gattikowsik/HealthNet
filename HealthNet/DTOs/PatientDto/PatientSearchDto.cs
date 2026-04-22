using System;
using HealthNetDb.Enums;
namespace HealthNet.DTOs.PatientDto;

public class PatientSearchDto
{
    public string? Name { get; set; }
    public PatientStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
