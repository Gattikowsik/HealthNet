using System;
using HealthNet.DTOs.AuditDTO;

namespace HealthNet.Services.AuditService;

public interface IAuditService
{
    Task<AuditResponseDto> AddAuditAsync(CreateAuditDto request, int userId);
    Task CloseAuditAsync(int auditId, int userId);
    Task<IEnumerable<AuditListDto>> GetAllAuditsAsync(AuditFilterDto filter); // NEW
    Task<AuditListDto> GetAuditByIdAsync(int auditId);                        // NEW
    Task UpdateAuditAsync(int auditId, UpdateAuditDto request);               // NEW
}
