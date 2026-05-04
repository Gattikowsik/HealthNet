using System.ComponentModel.DataAnnotations;

namespace HealthNet.DTOs.OutbreakMonitoringDTO
{
    public class AddEpidemiologyRequestDto
    {
        [Required]
        public string MetricsJSON { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public bool Status { get; set; } = true;
    }
}