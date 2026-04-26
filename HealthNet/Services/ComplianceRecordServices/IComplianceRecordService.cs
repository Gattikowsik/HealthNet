using System;
using HealthNet.DTOs.ComplianceRecordDto;

namespace HealthNet.Services.ComplianceRecordServices;

public interface IComplianceRecordService
{
    Task<ComplianceRecordResponseDto> AddComplianceRecordAsync(CreateComplianceRecordDto request, int userId);
    Task<IEnumerable<ComplianceRecordListDto>> GetAllComplianceRecordsAsync(ComplianceRecordFilterDto filter,int userId); 
}
