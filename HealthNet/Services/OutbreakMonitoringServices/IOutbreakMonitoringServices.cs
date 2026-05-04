using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;
namespace HealthNet.Services.OutbreakMonitoringServices;
public interface IOutbreakMonitoringServices
{
    Task<CreateOutbreakResponseDto> AddOutbreakService(int userId, CreateOutbreakRequestDto request);

    Task<UpdateOutbreakResponseDto> UpdateOutbreakService(int userId, int outbreakId, UpdateOutbreakRequestDto request);
    Task<GetOutbreakResponseDto?> GetOutbreakByIdService(int outbreakId);

}
