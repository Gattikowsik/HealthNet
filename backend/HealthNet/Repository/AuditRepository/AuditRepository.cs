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
            Scope     = request.Scope,
            Findings  = request.Findings,
            Status    = request.Status.Value,
            Date      = DateTime.UtcNow   // auto set to current date and time
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

    /// <summary>
    /// Closes an audit by setting Status = true (closed).
    /// </summary>
    /// <param name="auditId">The ID of the audit to close.</param>
    /// <returns>true if closed successfully, false if not found.</returns>
    public async Task CloseAuditAsync(int auditId)
    {
        // Find the audit by ID
        var audit = await _context.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);

        // Set Status to false = closed
        audit.Status = false;
        await _context.SaveChangesAsync();

    }
    public async Task<IEnumerable<AuditListDto>> GetAllAuditsAsync(AuditFilterDto filter)
    {
        // Start with all records — nothing hits DB yet
        var query = _context.Audits.AsQueryable();

        // Apply filters only if provided
        if (filter.AuditId.HasValue)
            query = query.Where(a => a.AuditId == filter.AuditId.Value);

        if (filter.OfficerId.HasValue)
            query = query.Where(a => a.OfficerId == filter.OfficerId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Scope))
            query = query.Where(a => a.Scope.ToLower().Contains(filter.Scope.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Findings))
            query = query.Where(a => a.Findings.ToLower().Contains(filter.Findings.ToLower()));

        if (filter.Date.HasValue)
            query = query.Where(a => a.Date.Date == filter.Date.Value.Date);

        // Hit DB here and map to DTO
        return await query.Select(a => new AuditListDto
        {
            AuditId   = a.AuditId,
            OfficerId = a.OfficerId,
            Scope     = a.Scope,
            Findings  = a.Findings,
            Date      = a.Date,
            Status    = a.Status
        }).ToListAsync();
    }

    /// <summary>
    /// Returns a single audit by ID.
    /// </summary>
    public async Task<AuditListDto?> GetAuditByIdAsync(int auditId)
    {
        return await _context.Audits
            .Where(a => a.AuditId == auditId)
            .Select(a => new AuditListDto
            {
                AuditId   = a.AuditId,
                OfficerId = a.OfficerId,
                Scope     = a.Scope,
                Findings  = a.Findings,
                Date      = a.Date,
                Status    = a.Status
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates Scope and Findings of an audit.
    /// </summary>
    public async Task UpdateAuditAsync(int auditId, UpdateAuditDto request)
    {
        var audit = await _context.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
        audit!.Scope    = request.Scope;
        audit!.Findings = request.Findings;
        await _context.SaveChangesAsync();
    }

}
