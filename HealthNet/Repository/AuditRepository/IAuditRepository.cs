using System;
using HealthNet.DTOs.AuditDTO;

namespace HealthNet.Repository.AuditRepository;

public interface IAuditRepository
{
    Task<AuditResponseDto> CreateAuditAsync(CreateAuditDto request, int userId);
    Task CloseAuditAsync(int auditId);
    Task<IEnumerable<AuditListDto>> GetAllAuditsAsync(AuditFilterDto filter); // NEW
    Task<AuditListDto?> GetAuditByIdAsync(int auditId);                       // NEW
    Task UpdateAuditAsync(int auditId, UpdateAuditDto request);
}
