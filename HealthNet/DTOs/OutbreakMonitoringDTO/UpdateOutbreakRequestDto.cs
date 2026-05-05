using System;
namespace HealthNet.DTOs.OutbreakMonitoringDTO;
public class UpdateOutbreakRequestDto
{
        public string Severity { get; set; } = null!;
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
 }

