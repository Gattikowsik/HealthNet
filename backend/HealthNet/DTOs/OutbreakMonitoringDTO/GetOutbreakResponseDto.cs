using System;
namespace HealthNet.DTOs.OutbreakMonitoringDTO;
public class GetOutbreakResponseDto
{
    public int OutbreakId { get; set; }
    public string? Disease { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Severity { get; set; }
    public bool Status { get; set; }
}
