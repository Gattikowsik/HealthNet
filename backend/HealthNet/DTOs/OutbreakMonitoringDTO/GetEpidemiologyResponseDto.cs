using System;

namespace HealthNet.DTOs.OutbreakMonitoringDTO;

public class GetEpidemiologyResponseDto
{

    public int EpiId { get; set; }
    public int OutbreakId { get; set; }
    public string MetricsJSON { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool Status { get; set; }
}


