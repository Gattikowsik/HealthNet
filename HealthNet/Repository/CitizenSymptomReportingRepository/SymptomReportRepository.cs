using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
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
}
