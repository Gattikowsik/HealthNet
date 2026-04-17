using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;

namespace HealthNet.Repository.OutbreakMonitoringRepository;

public interface IOutBreakMonitoringRepository
{
    Task<int> AddOutbreakAsync(CreateOutbreakRequestDto request);
    Task<bool> DuplicateOutbreakExitsAsync(CreateOutbreakRequestDto request);
}
