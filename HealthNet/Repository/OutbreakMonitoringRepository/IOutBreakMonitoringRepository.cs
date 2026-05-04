using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;
using HealthNetDb.Entities;
namespace HealthNet.Repository.OutbreakMonitoringRepository;

public enum UpdateOutbreakResult
{
    NotFound,
    NoChanges,
    Updated,
    Closed
}

public interface IOutBreakMonitoringRepository
{
    Task<int> AddOutbreakAsync(CreateOutbreakRequestDto request);
    Task<bool> DuplicateOutbreakExitsAsync(CreateOutbreakRequestDto request);
    Task<int> AddAuditLogAsync(int userId, string actionName, string resource);
    Task<Outbreak?> GetOutbreakByIdAsync(int outbreakId);
    Task<UpdateOutbreakResult> UpdateOutbreakAsync(int outbreakId, UpdateOutbreakRequestDto request);
    Task<bool> OutbreakExistsAsync(int outbreakId);
    Task<int> AddEpidemiologyAsync(Epidemiology epidemiology);
}
