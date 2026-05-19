using System;

namespace HealthNet.DTOs.OutbreakMonitoringDTO;

public class CreateOutbreakRequestDto
{
    public string Disease { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Severity { get; set; } = null!;
    public bool Status { get; set; }
}
