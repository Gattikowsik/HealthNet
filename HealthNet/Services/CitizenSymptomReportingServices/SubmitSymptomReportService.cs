using System;
using HealthNet.DTOs;
using HealthNet.DTOs.CitizenSymptomReportingDTO;
using HealthNet.DTOs.Pages;
using HealthNet.Repository;
using HealthNet.Services.PaginationService;
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

    public async Task<PagedResponseDto<SymptomReportResponseDto>> GetAllAsync(int userId, int pageNumber, int pageSize)
    {
        var query = _context.SymptomReports
                        .Include(r => r.Citizen)
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
}
