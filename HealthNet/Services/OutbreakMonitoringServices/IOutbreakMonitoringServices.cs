using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;

namespace HealthNet.Services.OutbreakMonitoringServices;

public interface IOutbreakMonitoringServices
{
    Task<CreateOutbreakResponseDto> AddOutbreakService(int userId,CreateOutbreakRequestDto request);
}
