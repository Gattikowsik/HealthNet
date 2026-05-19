using System;

namespace HealthNet.DTOs.AuditDTO;

public class AuditListDto
{
    public int      AuditId    { get; set; }
    public int      OfficerId  { get; set; }
    public string   Scope      { get; set; } = null!;
    public string   Findings   { get; set; } = null!;
    public DateTime Date       { get; set; }
    public bool     Status     { get; set; }  // true = open, false = closed
}
