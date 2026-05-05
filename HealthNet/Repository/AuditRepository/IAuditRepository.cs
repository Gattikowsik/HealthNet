using System;
using HealthNet.DTOs.AuditDTO;

namespace HealthNet.Repository.AuditRepository;

public interface IAuditRepository
{
    Task<AuditResponseDto> CreateAuditAsync(CreateAuditDto request, int userId);
    
}
