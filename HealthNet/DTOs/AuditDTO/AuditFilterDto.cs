using System;

namespace HealthNet.DTOs.AuditDTO;

public class AuditFilterDto
{
    public int?      AuditId    { get; set; }
    public int?      OfficerId  { get; set; }
    public string?   Scope      { get; set; }
    public string?   Findings   { get; set; }
    public DateTime? Date       { get; set; }
}
