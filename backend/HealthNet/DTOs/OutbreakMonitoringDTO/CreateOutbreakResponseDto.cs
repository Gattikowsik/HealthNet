using System;

namespace HealthNet.DTOs.OutbreakMonitoringDTO;

public class CreateOutbreakResponseDto
{
    public int OutbreakId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}
