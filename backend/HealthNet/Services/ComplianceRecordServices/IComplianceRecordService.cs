using System;
using HealthNet.DTOs.ComplianceRecordDto;

namespace HealthNet.Services.ComplianceRecordServices;

public interface IComplianceRecordService
{
    Task<ComplianceRecordResponseDto> AddComplianceRecordAsync(CreateComplianceRecordDto request, int userId);
    Task<IEnumerable<ComplianceRecordListDto>> GetAllComplianceRecordsAsync(ComplianceRecordFilterDto filter,int userId); 
    Task UpdateComplianceRecordAsync(int complianceId, UpdateComplianceRecordDto request);
    Task DeleteComplianceRecordAsync(int complianceId);   
    Task<ComplianceRecordListDto> GetComplianceRecordByIdAsync(int complianceId, int userId);                                
}
