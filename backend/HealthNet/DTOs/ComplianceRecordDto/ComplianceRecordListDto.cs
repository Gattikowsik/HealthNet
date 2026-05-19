using System;

namespace HealthNet.DTOs.ComplianceRecordDto;

public class ComplianceRecordListDto
{
    public int ComplianceId { get; set; }
    public int EntityId { get; set; }
    public string Type { get; set; } = null!;
    public string Result { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Notes { get; set; } = null!;
}
