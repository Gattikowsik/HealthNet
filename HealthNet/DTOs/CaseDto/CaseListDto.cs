using System;

namespace HealthNet.DTOs.CaseDto;

public class CaseListDto
{
    public int CaseId { get; set; }
    public int CitizenId { get; set; }
    public int DoctorId { get; set; }
    public string Diagnosis { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Status { get; set; }
}
