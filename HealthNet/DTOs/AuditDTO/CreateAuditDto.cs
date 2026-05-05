using System;

namespace HealthNet.DTOs.AuditDTO;

public class CreateAuditDto
{
    public string Scope { get; set; } = null!;       // what area is being audited
    public string Findings { get; set; } = null!;    // what was found
    public bool? Status { get; set; }                 // true = compliant, false = non compliant
}
