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

        // ── STEP 2: Scope should not be placeholder or pure number ─
        if (request.Scope.ToLower() == "string" || request.Scope.All(char.IsDigit))
            throw new ArgumentException(AuditHelper.InvalidScope);

        // ── STEP 3: Validate Findings ──────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Findings))
            throw new ArgumentException(AuditHelper.FindingsRequired);

        // ── STEP 4: Findings should not be placeholder or pure number
        if (request.Findings.ToLower() == "string" || request.Findings.All(char.IsDigit))
            throw new ArgumentException(AuditHelper.InvalidFindings);

        // ── STEP 5: Status must be explicitly provided ─────────────
        if (!request.Status.HasValue)
            throw new ArgumentException(AuditHelper.StatusRequired);


        // ── STEP 6: Check for duplicate ────────────────────────────
        // Reject if an open audit with the same Scope already exists for this officer
        var isDuplicate = await _context.Audits
            .AnyAsync(a => a.OfficerId == userId
                        && a.Scope.ToLower() == request.Scope.ToLower()
                        && a.Status == true);  // only reject if open record exists
        if (isDuplicate)
            throw new ArgumentException(AuditHelper.DuplicateAudit);

        try
        {
            // ── STEP 7: Save the audit record ──────────────────────
            var result = await _repository.CreateAuditAsync(request, userId);

            // ── STEP 8: Get ActionId for "Create" from Action table ─
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 9: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId = userId,
                ActionId = actionId,       // "Create" action
                Resource = "Audit",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // ── STEP 10: Return the AuditId ─────────────────────────
            return result;
        }
        catch
        {
            throw new Exception(AuditHelper.GenericError);
        }
    }

    // <summary>
    // Closes an audit by setting Status = true (closed)
    // </summary>
    // <param name="auditId"> ID of the audit to close </param>
    // <param name="officerId"> OfficerId extracted from JWT token </param>
    public async Task CloseAuditAsync(int auditId, int userId)
    {
        // ── STEP 1: Check if audit exists and is not already closed ─
        var audit = await _context.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);

        if (audit == null)
            throw new KeyNotFoundException(AuditHelper.AuditNotFound);

        if (audit.Status == false)
            throw new ArgumentException(AuditHelper.AuditAlreadyClosed);

        try
        {
            // ── STEP 2: Close the audit via repository ──────────────
            await _repository.CloseAuditAsync(auditId);

            // ── STEP 3: Get ActionId for "Update" ──────────────────
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Update")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 4: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId = userId,
                ActionId = actionId,       // "Update" action
                Resource = "Audit",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception(AuditHelper.GenericError);
        }
    }
    public async Task<IEnumerable<AuditListDto>> GetAllAuditsAsync(AuditFilterDto filter)
    {
        var audits = await _repository.GetAllAuditsAsync(filter);

        if (!audits.Any())
            throw new KeyNotFoundException(AuditHelper.NoAuditsFound);

        return audits;
    }

    // <summary>
    // Returns a single audit by ID
    // </summary>
    public async Task<AuditListDto> GetAuditByIdAsync(int auditId)
    {
        var audit = await _repository.GetAuditByIdAsync(auditId);

        if (audit == null)
            throw new KeyNotFoundException(AuditHelper.AuditNotFound);

        return audit;
    }

    // <summary>
    // Updates Scope and Findings
    // Blocked if Status = false (closed)
    // </summary>
    public async Task UpdateAuditAsync(int auditId, UpdateAuditDto request)
    {
        // ── STEP 1: Validate Scope ─────────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Scope))
            throw new ArgumentException(AuditHelper.ScopeRequired);

        if (request.Scope.ToLower() == "string" || request.Scope.All(char.IsDigit))
            throw new ArgumentException(AuditHelper.InvalidScope);

        // ── STEP 2: Validate Findings ──────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Findings))
            throw new ArgumentException(AuditHelper.FindingsRequired);

        if (request.Findings.ToLower() == "string" || request.Findings.All(char.IsDigit))
            throw new ArgumentException(AuditHelper.InvalidFindings);

        // ── STEP 3: Check if audit exists ──────────────────────────
        var audit = await _context.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
        if (audit == null)
            throw new KeyNotFoundException(AuditHelper.AuditNotFound);

        // ── STEP 4: Cannot update if closed ────────────────────────
        if (audit.Status == false)
            throw new ArgumentException(AuditHelper.AuditAlreadyClosed);

        try
        {
            await _repository.UpdateAuditAsync(auditId, request);
        }
        catch
        {
            throw new Exception(AuditHelper.GenericError);
        }
    }

}
