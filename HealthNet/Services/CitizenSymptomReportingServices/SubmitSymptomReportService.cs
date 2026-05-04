using System;
using HealthNet.DTOs;
using HealthNet.DTOs.CitizenSymptomReportingDTO;
using HealthNet.DTOs.Pages;
using HealthNet.Repository;
using HealthNet.Services.PaginationService;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNet.Services;
public class SubmitSymptomReportService : ISubmitSymptomReportService
{
    private readonly ISubmitSymptomReportRepository _repository;
    private readonly HealthNetContext _context;
    private readonly IPaginationService _paginationService;
    public SubmitSymptomReportService(ISubmitSymptomReportRepository repository, HealthNetContext context, IPaginationService paginationService)
    {
        _repository = repository;
        _context = context;
        _paginationService = paginationService;
    }

    public async Task<PagedResponseDto<SymptomReportResponseDto>> GetAllAsync(int userId, int? citizenId, DateTime? reportDate, SymptomStatus? status, int pageNumber, int pageSize)
    {
        // If citizenId filter is provided, validate citizen existence
        if (citizenId.HasValue)
        {
            var citizenExists = await _context.Userss
                .AnyAsync(u => u.UserId == citizenId.Value);

            if (!citizenExists)
                throw new HealthNetException("No such citizen exists.");
        }
        var baseQuery = _context.SymptomReports
            .Include(r => r.Citizen)
            .AsQueryable();

        //  FILTERS (entity level)
        if (citizenId.HasValue)
            baseQuery = baseQuery.Where(r => r.CitizenId == citizenId.Value);

        if (reportDate.HasValue)
            baseQuery = baseQuery.Where(r => r.Date.Date == reportDate.Value.Date);

        if (status.HasValue && Enum.IsDefined(typeof(SymptomStatus), status.Value))
            baseQuery = baseQuery.Where(r => r.Status == status.Value);

        //  PROJECTION AFTER FILTERING
        var query = baseQuery
            .OrderBy(r => r.ReportId)
            .Select(r => new SymptomReportResponseDto
            {
                ReportId = r.ReportId,
                CitizenId = r.CitizenId,
                CitizenName = r.Citizen.Name,
                SymptomsJson = r.SymptomsJson,
                Date = r.Date,
                Status = r.Status.ToString()
            });

        //  PAGINATION LAST
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        if (result.TotalRecords == 0)
        {
            result.Message = "No symptom reports found for the given filter criteria.";
        }


        //  AUDIT LOG
        var readActionId = await _context.Actions
            .Where(a => a.ActionName == "READ")
            .Select(a => a.ActionId)
            .FirstAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionId = readActionId,
            Resource = "SymptomReport",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<PagedResponseDto<SymptomReportResponseDto>>GetMineAsync(int userId, int pageNumber, int pageSize)
    {
        var query = _context.SymptomReports
                        .Where(r => r.CitizenId == userId)
                        .OrderBy(r => r.ReportId)
                        .Select(r => new SymptomReportResponseDto
                        {
                            ReportId = r.ReportId,
                            SymptomsJson = r.SymptomsJson,
                            Date = r.Date,
                            Status = r.Status.ToString()
                        });
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // FETCH READ ACTION ID dynamically
        var readActionId = await _context.Actions
            .Where(a => a.ActionName == "READ")
            .Select(a => a.ActionId)
            .FirstAsync();

        // AUDIT LOG ENTRY
        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = readActionId,     // dynamic, not hardcoded
            Resource = "SymptomReport",
            Timestamp = DateTime.UtcNow
        };
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<SubmitSymptomReportResponseDto> SubmitAsync(SubmitSymptomReportRequestDto request, int citizenId)
    {

        // Centralized validation
        SymptomReportValidator.ValidateSymptomsJson(request.SymptomsJson);
        SymptomReportValidator.ValidateDate(request.Date);

        // Duplicate check (block until CLOSED)
        var existingActiveReport = await _context.SymptomReports
            .Where(r =>
                r.CitizenId == citizenId &&
                r.SymptomsJson == request.SymptomsJson &&
                r.Date.Date == request.Date.Date &&
                r.Status != SymptomStatus.Closed)
            .FirstOrDefaultAsync();

        if (existingActiveReport != null)
        {
            throw new InvalidOperationException(
                "A symptom report with the same details already exists and is not closed.");
        }
        var report = new SymptomReport
        {
            CitizenId = citizenId,
            SymptomsJson = request.SymptomsJson,
            Date = request.Date,
            Status = SymptomStatus.Submitted
        };
        var saved = await _repository.AddAsync(report);

        //Fetch ActionId for CREATE
        var actionId = await _context
        .Set<HealthNetDb.Entities.Action>()
        .Where(a => a.ActionName == "CREATE")
        .Select(a => a.ActionId)
        .FirstAsync();

        //Create AuditLog entry
        var auditLog = new AuditLog
        {
            UserId = citizenId,
            ActionId = actionId,
            Resource = "SymptomReport",
            Timestamp = DateTime.UtcNow
        };

        //Save AuditLog
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        return new SubmitSymptomReportResponseDto
        {
            ReportId = saved.ReportId
        };
    }
    public async Task<bool> UpdateStatusAsync(int reportId, SymptomStatus newStatus, int userId)
    {
        // ❗ Validate status
        if (!Enum.IsDefined(typeof(SymptomStatus), newStatus))
            throw new ArgumentException("Invalid status value.");

        var report = await _context.SymptomReports
            .FirstOrDefaultAsync(r => r.ReportId == reportId);

        if (report == null)
            return false;

        report.Status = newStatus;

        // AUDIT LOG: UPDATE
        var updateActionId = await _context.Actions
            .Where(a => a.ActionName == "UPDATE")
            .Select(a => a.ActionId)
            .FirstAsync();

        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = updateActionId,
            Resource = "SymptomReport",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();
        return true;
    }

}
