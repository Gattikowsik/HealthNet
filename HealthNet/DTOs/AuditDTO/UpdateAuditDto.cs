using System;

namespace HealthNet.DTOs.AuditDTO;

public class UpdateAuditDto
{
    public string Scope    { get; set; } = null!;
    public string Findings { get; set; } = null!;
}
