using System;
using HealthNet.DTOs;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;
namespace HealthNet.Services;
using HealthNet.DTOs.CitizenSymptomReportingDTO;
using HealthNet.DTOs.Pages;
public interface ISubmitSymptomReportService
{
    Task<SubmitSymptomReportResponseDto> SubmitAsync(SubmitSymptomReportRequestDto request, int citizenId);
    Task<PagedResponseDto<SymptomReportResponseDto>>GetMineAsync(int userId, int pageNumber, int pageSize);
    Task<PagedResponseDto<SymptomReportResponseDto>>GetAllAsync(int userId, int? citizenId, DateTime? reportDate, 
        SymptomStatus? status, int pageNumber, int pageSize);
    Task<bool> UpdateStatusAsync(int reportId, SymptomStatus newStatus, int userId);

}
