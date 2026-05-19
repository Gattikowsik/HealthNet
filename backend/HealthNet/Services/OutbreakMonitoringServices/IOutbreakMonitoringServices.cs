using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;
namespace HealthNet.Services.OutbreakMonitoringServices;

public interface IOutbreakMonitoringServices
{
    Task<CreateOutbreakResponseDto> AddOutbreakService(int userId, CreateOutbreakRequestDto request);

    Task<UpdateOutbreakResponseDto> UpdateOutbreakService(int userId, int outbreakId, UpdateOutbreakRequestDto request);
    Task<GetOutbreakResponseDto?> GetOutbreakByIdService(int outbreakId);
    Task<AddEpidemiologyResponseDto> AddEpidemiologyService(int userId, int outbreakId, AddEpidemiologyRequestDto request);
    Task<List<GetEpidemiologyResponseDto>> GetAllEpidemiologyService();
    Task<GetEpidemiologyResponseDto?> GetEpidemiologyByIdService(int epiId);
    Task<UpdateEpidemiologyResponseDto> UpdateEpidemiologyService(int userId, int epiId, UpdateEpidemiologyRequestDto request);
    Task<List<GetActiveOutbreaksResponseDto>> GetAllActiveOutbreaksService();

    Task<DeleteResponseDto> DeleteOutbreakService(int userId, int outbreakId);
    Task<DeleteResponseDto> DeleteEpidemiologyService(int userId, int epiId);

}
