using System;
using HealthNet.DTOs.AuditDTO;

namespace HealthNet.Services.AuditService;

public interface IAuditService
{
    Task<AuditResponseDto> AddAuditAsync(CreateAuditDto request, int userId);
}
