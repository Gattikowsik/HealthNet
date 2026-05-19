using System;
using HealthNet.DTOs.CaseDto;
namespace HealthNet.Services.CaseService;

public interface ICasesService
{
    Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId);
    Task<IEnumerable<CaseListDto>> GetAllCasesAsync();
    Task<CaseListDto> GetCaseByIdAsync(int caseId);
    Task UpdateCaseDiagnosisAsync(int caseId, UpdateCaseDiagnosisDto request, int doctorId);
    Task DeleteCaseAsync(int caseId, int doctorId);
}
