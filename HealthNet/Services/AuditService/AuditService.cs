using System;
using HealthNet.DTOs.AuditDTO;
using HealthNet.Repository.AuditRepository;
using HealthNet.Utility;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Services.AuditService;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _repository;
    private readonly HealthNetContext _context;

    public AuditService(IAuditRepository repository, HealthNetContext context)
    {
        _repository = repository;
        _context = context;
    }
    // <summary>
    // Officer records audit findings
    // </summary>
    // <param name="request"> Audit Request DTO for data transfer from client </param>
    // <param name="officerId"> OfficerId extracted from the JWT token of the logged in user </param>
    // <returns> AuditResponseDto containing the generated AuditId </returns>

    public async Task<AuditResponseDto> AddAuditAsync(CreateAuditDto request, int userId)
    {
        // ── STEP 1: Validate Scope ─────────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Scope))
            throw new ArgumentException(AuditHelper.ScopeRequired);

        // ── STEP 2: Validate Findings ──────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Findings))
            throw new ArgumentException(AuditHelper.FindingsRequired);

        // ── STEP 3: Check for duplicate ────────────────────────────
        var isDuplicate = await _context.Audits
            .AnyAsync(a => a.OfficerId == userId
                        && a.Scope.ToLower() == request.Scope.ToLower());
        if (isDuplicate)
            throw new ArgumentException(AuditHelper.DuplicateAudit);

        try
        {
            // ── STEP 4: Save the audit record ──────────────────────
            var result = await _repository.CreateAuditAsync(request, userId);

            // ── STEP 5: Get ActionId for "Create" from Action table ─
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 6: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId    = userId,
                ActionId  = actionId,       // "Create" action
                Resource  = "Audit",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // ── STEP 7: Return the AuditId ─────────────────────────
            return result;
        }
        catch
        {
            throw new Exception(AuditHelper.GenericError);
        }
    }

}
