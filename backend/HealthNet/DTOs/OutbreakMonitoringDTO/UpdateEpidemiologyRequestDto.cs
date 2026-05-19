using System;
namespace HealthNet.DTOs.OutbreakMonitoringDTO
{
    public class UpdateEpidemiologyRequestDto
    {
        public string MetricsJSON { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
