using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNet.Repository;

public class SubmitSymptomReportRepository : ISubmitSymptomReportRepository
{
    private readonly HealthNetContext _context;
    public SubmitSymptomReportRepository(HealthNetContext context)
    {
        _context = context;
    }
    public async Task<SymptomReport> AddAsync(SymptomReport report)
    {
        _context.SymptomReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    // For Doctors / Researchers / Admin → Get ALL reports (with pagination)
    public async Task<(List<SymptomReport>, int)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.SymptomReports.AsQueryable();

        var totalCount = await query.CountAsync();

        var reports = await query
            .OrderByDescending(r => r.Date)                // newest first
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reports, totalCount);
    }

    //  For Citizen → Get ONLY their reports (with pagination)
    public async Task<(List<SymptomReport>, int)> GetMineAsync(
        int citizenId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.SymptomReports
            .Where(r => r.CitizenId == citizenId);

        var totalCount = await query.CountAsync();

        var reports = await query
            .OrderByDescending(r => r.Date)               // newest first
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reports, totalCount);
    }
}
