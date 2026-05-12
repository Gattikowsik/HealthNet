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

        // ── STEP 4: Check if EntityId exists in respective table ───
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
            // ── STEP 5: Save the compliance record ─────────────────
            var result = await _repository.CreateComplianceRecordAsync(request);

            // ── STEP 6: Log to AuditLog ────────────────────────────
            // ActionId = 2 because "Create" is the second action in the Action table
            // Resource is hardcoded as "ComplianceRecord"
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 7: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId = userId,
                ActionId = actionId,
                Resource = "ComplianceRecord",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // ── STEP 8: Return the ComplianceId ────────────────────
            return result;
        }
        catch
        {
            throw new Exception(ComplianceHelper.GenericError);
        }
    }

    // <summary>
    // Returns a filtered list of compliance records
    // </summary>
    // <param name="filter"> Filter DTO containing optional filters </param>
    // <returns> List of ComplianceRecordListDto matching the filters </returns>
    public async Task<IEnumerable<ComplianceRecordListDto>> GetAllComplianceRecordsAsync(ComplianceRecordFilterDto filter,int userId)
    {
        // ── STEP 1: Validate Type if provided ──────────────────────
        var allowedTypes = new[] { "case", "test", "outbreak" };
        if (!string.IsNullOrWhiteSpace(filter.Type)
            && !allowedTypes.Contains(filter.Type.ToLower()))
            throw new ArgumentException(ComplianceHelper.InvalidType);

        // ── STEP 2: Validate Result if provided ────────────────────
        var allowedResults = new[] { "compliant", "non compliant", "partially compliant", "pending review" };
        if (!string.IsNullOrWhiteSpace(filter.Result)
            && !allowedResults.Contains(filter.Result.ToLower()))
            throw new ArgumentException(ComplianceHelper.InvalidResult);

        // ── STEP 3: Fetch from repository ──────────────────────────
        var records = await _repository.GetComplianceRecordsAsync(filter);

        // ── STEP 4: Check if any records were found ─────────────────
        if (!records.Any())
            throw new KeyNotFoundException(ComplianceHelper.NoRecordsFound);

        try
        {
            // ── STEP 5: Get ActionId for "Read" from Action table ───
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Read")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 6: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId    = userId,
                ActionId  = actionId,       
                Resource  = "ComplianceRecord",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception(ComplianceHelper.GenericError);
        }

        // ── STEP 7: Return the records ─────────────────────────────
        return records;
    }

    /// <summary>
    /// Updates the Result and Notes of a compliance record. Cannot update if record is compliant or deleted.   
    /// </summary>
    /// <param name="complianceId"></param>
    /// <param name="request"></param>
    public async Task UpdateComplianceRecordAsync(int complianceId, UpdateComplianceRecordDto request)
    {
        // ── STEP 1: Validate Result ────────────────────────────────
        var allowedResults = new[] { "non compliant", "partially compliant", "pending review" };
        if (!allowedResults.Contains(request.Result.ToLower()))
            throw new ArgumentException(ComplianceHelper.InvalidResult);

        // ── STEP 2: Validate Notes ─────────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Notes))
            throw new ArgumentException(ComplianceHelper.NotesRequired);

        // ── STEP 3: Check if record exists ─────────────────────────
        var record = await _context.ComplianceRecords
            .FirstOrDefaultAsync(c => c.ComplianceId == complianceId);
        if (record == null)
            throw new KeyNotFoundException(ComplianceHelper.RecordNotFound);

        // ── STEP 4: Cannot update if deleted ───────────────────────
        if (record.IsDeleted)
            throw new ArgumentException(ComplianceHelper.CannotUpdateDeleted);

        // ── STEP 5: Cannot update if compliant ─────────────────────
        if (record.Result == "compliant")
            throw new ArgumentException(ComplianceHelper.CannotUpdateCompliant);

        try
        {
            await _repository.UpdateComplianceRecordAsync(complianceId, request);
        }
        catch
        {
            throw new Exception(ComplianceHelper.GenericError);
        }
    }

    /// <summary>
    /// Soft deletes a compliance record by setting IsDeleted to true. Cannot delete if already deleted.
    /// </summary>
    /// <param name="complianceId"></param>
    public async Task DeleteComplianceRecordAsync(int complianceId)
    {
        // ── STEP 1: Check if record exists ─────────────────────────
        var record = await _context.ComplianceRecords
            .FirstOrDefaultAsync(c => c.ComplianceId == complianceId);
        if (record == null)
            throw new KeyNotFoundException(ComplianceHelper.RecordNotFound);

        // ── STEP 2: Cannot delete if already deleted ───────────────
        if (record.IsDeleted)
            throw new ArgumentException(ComplianceHelper.AlreadyDeleted);

        try
        {
            await _repository.DeleteComplianceRecordAsync(complianceId);
        }
        catch
        {
            throw new Exception(ComplianceHelper.GenericError);
        }
    }
}
