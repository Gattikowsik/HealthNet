using System;
using HealthNet.DTOs.ComplianceRecordDto;
using HealthNet.Repository.ComplianceRecord;
using Microsoft.EntityFrameworkCore;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using System.Security.Claims;

namespace HealthNet.Services.ComplianceRecordServices;

public class ComplianceRecordService : IComplianceRecordService
{
    private readonly IComplianceRepository _repository;
    private readonly HealthNetContext _context;

    public ComplianceRecordService(IComplianceRepository repository, HealthNetContext context)
    {
        _repository = repository;
        _context = context;
    }

    // <summary>
    // Logs a compliance check result for a Case, LabTest or Outbreak
    // </summary>
    // <param name="request"> Compliance Record Request DTO for data transfer from client </param>
    // <param name="userId"> UserId extracted from the JWT token of the logged in user </param>
    // <returns> ComplianceRecordResponseDto containing the generated ComplianceId </returns>
    public async Task<ComplianceRecordResponseDto> AddComplianceRecordAsync(CreateComplianceRecordDto request, int userId)
    {
        // ── STEP 1: Validate the Type ──────────────────────────────
        var allowedTypes = new[] { "case", "test", "outbreak" };
        if (!allowedTypes.Contains(request.Type.ToLower()))
            throw new ArgumentException(ComplianceHelper.InvalidType);

        // ── STEP 2: Validate the Result ────────────────────────────
        var allowedResults = new[] { "compliant", "non compliant", "partially compliant", "pending review" };
        if (!allowedResults.Contains(request.Result.ToLower()))
            throw new ArgumentException(ComplianceHelper.InvalidResult);


        // ── STEP 3: Check for duplicate compliance record ──────
        var isDuplicate = await _context.ComplianceRecords
            .AnyAsync(c => c.EntityId == request.EntityId 
                        && c.Type == request.Type.ToLower());
        if (isDuplicate)
            throw new ArgumentException(ComplianceHelper.DuplicateRecord);    

        // ── STEP 3: Check if EntityId exists in respective table ───
        bool entityExists = request.Type.ToLower() switch
        {
            "case" => await _context.Casess.AnyAsync(c => c.CaseId == request.EntityId),
            "test" => await _context.LabTests.AnyAsync(t => t.TestId == request.EntityId),
            "outbreak" => await _context.Outbreaks.AnyAsync(o => o.OutbreakId == request.EntityId),
            _ => false
        };
        if (!entityExists)
            throw new ArgumentException(ComplianceHelper.EntityNotFound);

        try
        {
            // ── STEP 4: Save the compliance record ─────────────────
            var result = await _repository.CreateComplianceRecordAsync(request);

            // ── STEP 5: Log to AuditLog ────────────────────────────
            // ActionId = 2 because "Create" is the second action in the Action table
            // Resource is hardcoded as "ComplianceRecord"
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 6: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId = userId,
                ActionId = actionId,
                Resource = "ComplianceRecord",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // ── STEP 7: Return the ComplianceId ────────────────────
            return result;
        }
        catch
        {
            throw new Exception(ComplianceHelper.GenericError);
        }
    }
}
