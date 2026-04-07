using System;
using HealthNet.DTOs;
using HealthNet.Repository;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNet.Services;
public class SymptomReportService : ISymptomReportService
{
private readonly ISymptomReportRepository _repository;
private readonly HealthNetContext _context;
public SymptomReportService(ISymptomReportRepository repository,HealthNetContext context)
{
     _repository = repository;
     _context = context;
}
    public async Task<SymptomReportResponseDto> SubmitAsync(SymptomReportRequestDto request, int citizenId)
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

        return new SymptomReportResponseDto
        {
            ReportId = saved.ReportId
        };
    }
}
