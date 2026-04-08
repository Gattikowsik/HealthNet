using System;

namespace HealthNet.DTOs.ComplianceRecordDto;

public class CreateComplianceRecordDto
{
    // Which case/test/outbreak are they checking?
    public int EntityId { get; set; }
    // Maybe the officer is checking a case, a test, or an outbreak. This field specifies which one.
    public string Type { get; set; } = null!;
    // What was the outcome? e.g. "resolved", "pending", "non-compliant"
    public string Result { get; set; } = null!;
    // When was this compliance check done?
    public DateTime Date { get; set; }
    // Any extra details the officer wants to note
    public string Notes { get; set; } = null!;
}
