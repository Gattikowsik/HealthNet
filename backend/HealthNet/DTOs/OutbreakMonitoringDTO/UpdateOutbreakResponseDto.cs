using System;
namespace HealthNet.DTOs.OutbreakMonitoringDTO;
public class UpdateOutbreakResponseDto
{
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
}
