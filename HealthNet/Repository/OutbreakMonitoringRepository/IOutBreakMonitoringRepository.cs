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
    Task<bool> EpidemiologyDuplicateExistsAsync(int outbreakId, DateTime date);
    Task<int> AddEpidemiologyAsync(Epidemiology epidemiology);
    Task<List<Epidemiology>> GetAllEpidemiologyAsync();
    Task<Epidemiology?> GetEpidemiologyByIdAsync(int epiId);
    Task<bool> UpdateEpidemiologyAsync(int epiId, UpdateEpidemiologyRequestDto request);
    Task<List<Outbreak>> GetAllActiveOutbreaksAsync();
    Task<bool> DeleteOutbreakAsync(int outbreakId);
    Task<bool> SoftDeleteEpidemiologyAsync(int epiId);

}
