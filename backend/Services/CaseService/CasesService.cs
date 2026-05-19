using System;
using HealthNet.DTOs.CaseDto;
using HealthNet.Repository.CaseRepository;
using Microsoft.EntityFrameworkCore;
using HealthNet.Utility;
using HealthNetDb.Data;
namespace HealthNet.Services.CaseService;

public class CasesService : ICasesService
{
    private readonly ICasesRepository _repository;
    private readonly HealthNetContext _context;

    public CasesService(ICasesRepository repository, HealthNetContext context)
    {
        _repository = repository;
        _context = context;
    }

    // <summary>
    // Doctor creates a case from a symptom report
    // </summary>
    // <param name="request"> Case Request DTO from client </param>
    // <param name="doctorId"> DoctorId extracted from JWT token </param>
    // <returns> CaseResponseDto containing the generated CaseId </returns>
    public async Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId)
    {
        // ── STEP 1: Validate Diagnosis ─────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Diagnosis))
            throw new ArgumentException(CasesHelper.DiagnosisRequired);

        // ── STEP 2: Diagnosis should not be placeholder or number ──
        if (request.Diagnosis.ToLower() == "string" || request.Diagnosis.All(char.IsDigit))
            throw new ArgumentException(CasesHelper.InvalidDiagnosis);

        // ── STEP 3: Status must be explicitly provided ─────────────
        if (!request.Status.HasValue)
            throw new ArgumentException(CasesHelper.StatusRequired);

        // ── STEP 4: Check if ReportId exists in SymptomReport ──────
        var report = await _context.SymptomReports
            .FirstOrDefaultAsync(r => r.ReportId == request.ReportId);
        if (report == null)
            throw new ArgumentException(CasesHelper.ReportNotFound);

        // ── STEP 5: Check for duplicate case for same citizen ──────
        var isDuplicate = await _context.Casess
            .AnyAsync(c => c.CitizenId == report.CitizenId);
        if (isDuplicate)
            throw new ArgumentException(CasesHelper.DuplicateCase);

        try
        {
            // ── STEP 6: Get CitizenId from SymptomReport ───────────
            int citizenId = report.CitizenId!.Value;

            // ── STEP 7: Save the case ──────────────────────────────
            var result = await _repository.CreateCaseAsync(request, doctorId, citizenId);

            // ── STEP 8: Get ActionId for "Create" ──────────────────
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 9: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId    = doctorId,
                ActionId  = actionId,
                Resource  = "Cases",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return result;
        }
        catch
        {
            throw new Exception(CasesHelper.GenericError);
        }
    }

    // <summary>
    // Get all cases
    // </summary>
    public async Task<IEnumerable<CaseListDto>> GetAllCasesAsync()
    {
        var cases = await _repository.GetAllCasesAsync();

        if (!cases.Any())
            throw new KeyNotFoundException(CasesHelper.CaseNotFound);

        return cases;
    }

    // <summary>
    // Get a case by ID
    // </summary>
    public async Task<CaseListDto> GetCaseByIdAsync(int caseId)
    {
        var cases = await _repository.GetCaseByIdAsync(caseId);

        if (cases == null)
            throw new KeyNotFoundException(CasesHelper.CaseNotFound);

        return cases;
    }

    // <summary>
    // Doctor updates only the diagnosis
    // Only allowed when Status = false (Under Treatment)
    // </summary>
    public async Task UpdateCaseDiagnosisAsync(int caseId, UpdateCaseDiagnosisDto request, int doctorId)
    {
        // ── STEP 1: Validate Diagnosis ─────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Diagnosis))
            throw new ArgumentException(CasesHelper.DiagnosisRequired);

        if (request.Diagnosis.ToLower() == "string" || request.Diagnosis.All(char.IsDigit))
            throw new ArgumentException(CasesHelper.InvalidDiagnosis);

        // ── STEP 2: Check if case exists ───────────────────────────
        var cases = await _context.Casess
            .FirstOrDefaultAsync(c => c.CaseId == caseId);
        if (cases == null)
            throw new KeyNotFoundException(CasesHelper.CaseNotFound);

        // ── STEP 3: Cannot update if already recovered ─────────────
        if (cases.Status == true)
            throw new ArgumentException(CasesHelper.CaseAlreadyRecovered);

        try
        {
            // ── STEP 4: Update diagnosis ───────────────────────────
            await _repository.UpdateCaseDiagnosisAsync(caseId, request.Diagnosis);

            // ── STEP 5: Get ActionId for "Update" ──────────────────
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Update")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 6: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId    = doctorId,
                ActionId  = actionId,
                Resource  = "Cases",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception(CasesHelper.GenericError);
        }
    }

    // <summary>
    // Doctor deletes a case
    // System automatically sets Status = true (Recovered) then deletes
    // </summary>
    public async Task DeleteCaseAsync(int caseId, int doctorId)
    {
        // ── STEP 1: Check if case exists ───────────────────────────
        var cases = await _context.Casess
            .FirstOrDefaultAsync(c => c.CaseId == caseId);
        if (cases == null)
            throw new KeyNotFoundException(CasesHelper.CaseNotFound);

        try
        {
            // ── STEP 2: Set Status = true then delete ───────────────
            await _repository.DeleteCaseAsync(caseId);

            // ── STEP 3: Get ActionId for "Delete" ──────────────────
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Delete")
                .Select(a => a.ActionId)
                .FirstAsync();

            // ── STEP 4: Log to AuditLog ────────────────────────────
            var auditLog = new HealthNetDb.Entities.AuditLog
            {
                UserId    = doctorId,
                ActionId  = actionId,
                Resource  = "Cases",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception(CasesHelper.GenericError);
        }
    }
}
