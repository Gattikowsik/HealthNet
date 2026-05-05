using System;

namespace HealthNet.Utility;

public class EpidemiologyResponse
{
    public int EpiId { get; set; }
    public string? MetricsJSON { get; set; }
    public DateTime EpiDate { get; set; }
    public string? Disease { get; set; }
    public string? Location { get; set; }
    public DateTime OutbreakStartDate { get; set; }
    public DateTime OutbreakEndDate { get; set; }
    public string? Severity { get; set; }
}
