using System;
using HealthNet.DTOs;
namespace HealthNet.Services;
public interface ISubmitSymptomReportService
{
    Task<SubmitSymptomReportResponseDto> SubmitAsync(SubmitSymptomReportRequestDto request, int citizenId);
}
