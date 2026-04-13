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
                        .OrderByDescending(r => r.Date)
                        .Select(r => new SymptomReportResponseDto
                        {
                            ReportId = r.ReportId,
                            SymptomsJson = r.SymptomsJson,
                            Date = r.Date,
                            Status = r.Status
                        });
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // AUDIT LOG (READ)
        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = 7,               // READ
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
                        .OrderByDescending(r => r.Date)
                        .Select(r => new SymptomReportResponseDto
                        {
                            ReportId = r.ReportId,
                            SymptomsJson = r.SymptomsJson,
                            Date = r.Date,
                            Status = r.Status
                        });
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // AUDIT LOG (READ)
        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = 7,               // READ
            Resource = "SymptomReport",
            Timestamp = DateTime.UtcNow
        };
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<SubmitSymptomReportResponseDto> SubmitAsync(SubmitSymptomReportRequestDto request, int citizenId)
    {
        var report = new SymptomReport
        {
            CitizenId = citizenId,
            SymptomsJson = request.SymptomsJson,
            Date = request.Date,
            Status = true
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
