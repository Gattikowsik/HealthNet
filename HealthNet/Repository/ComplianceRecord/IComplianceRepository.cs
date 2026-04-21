using System;
using HealthNet.DTOs.ComplianceRecordDto;
using HealthNetDb.Entities;

namespace HealthNet.Repository.ComplianceRecord;

public interface IComplianceRepository
{
    Task<ComplianceRecordResponseDto> CreateComplianceRecordAsync(CreateComplianceRecordDto complianceRecord);

    Task<IEnumerable<ComplianceRecordListDto>> GetComplianceRecordsAsync(ComplianceRecordFilterDto filter);
}
