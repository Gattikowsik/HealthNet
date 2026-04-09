using System;
using HealthNetDb.Enums;
namespace HealthNet.DTOs.PateintDto;

public class PatientSearchDto
{
    public string? Name { get; set; }
    public PatientStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

}
