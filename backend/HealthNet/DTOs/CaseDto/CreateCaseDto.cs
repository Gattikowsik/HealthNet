using System;

namespace HealthNet.DTOs.CaseDto;

public class CreateCaseDto
{
    public int ReportId { get; set; }       // SymptomReport ID — CitizenId is fetched from this
    public string Diagnosis { get; set; } = null!;
    public bool? Status { get; set; }       // true = Recovered, false = Under Treatment
                                            
}
