using System;
using HealthNet.DTOs;
namespace HealthNet.Services;
public interface ISymptomReportService
{    
Task<SymptomReportResponseDto> SubmitAsync(SymptomReportRequestDto request,int citizenId);
}
