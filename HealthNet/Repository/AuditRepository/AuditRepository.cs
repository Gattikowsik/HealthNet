using System;
using HealthNet.DTOs.AuditDTO;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.AuditRepository;

public class AuditRepository : IAuditRepository
{
    HealthNetContext _context;

    public AuditRepository(HealthNetContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Creates a new audit record in the database.
    /// </summary>
    /// <param name="request">The DTO containing the audit data.</param>
    /// <param name="officerId">The ID of the officer extracted from JWT token.</param>
    /// <returns>AuditResponseDto containing the generated AuditId.</returns>
    public async Task<AuditResponseDto> CreateAuditAsync(CreateAuditDto request, int userId)
    {
        // Map DTO → Entity
        var audit = new HealthNetDb.Entities.Audit
        {
            OfficerId = userId,        // from JWT token
            Scope = request.Scope,
            Findings = request.Findings,
            Status = request.Status.Value,
            Date = DateTime.UtcNow   // auto set to current date and time
        };

        // Save to DB — EF Core generates AuditId after this
        _context.Audits.Add(audit);
        await _context.SaveChangesAsync();

        // Return the generated AuditId
        return new AuditResponseDto
        {
            AuditId = audit.AuditId
        };
    }

    

}
