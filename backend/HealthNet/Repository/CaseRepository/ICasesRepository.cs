using System;
using HealthNet.DTOs.CaseDto;
namespace HealthNet.Repository.CaseRepository;

public interface ICasesRepository
{
    Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId, int citizenId);
    Task<IEnumerable<CaseListDto>> GetAllCasesAsync();
    Task<CaseListDto?> GetCaseByIdAsync(int caseId);
    Task UpdateCaseDiagnosisAsync(int caseId, string diagnosis);
    Task DeleteCaseAsync(int caseId);
}
