using System;

namespace HealthNet.DTOs.OutbreakMonitoringDTO;

public class LocationResponseDto
{
    public string? DisplayName { get; set; }
    public string? Lon { get; set; }
    public string? Lat { get; set; }
}
