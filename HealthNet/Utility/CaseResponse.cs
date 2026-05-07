using System;

namespace HealthNet.Utility;

public class CaseResponse
{
    public int CaseId { get; set; }
    public string? CitizenName { get; set; }
    public string? DoctorName { get; set; }
    public string? Diagnosis { get; set; }
    public DateTime RegisteredDate { get; set; }
    public string? Status { get; set; }

}
