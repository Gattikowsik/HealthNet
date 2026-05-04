using System;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.ReportingAndAnalytics;

public class ReportingAndAnalyticsRepository : IReportingAndAnalyticsRepository
{
    private readonly HealthNetContext _context;
    public ReportingAndAnalyticsRepository(HealthNetContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get Outbreaks from the Database
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  The Outbreaks from the DB
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<OutbreakAnalyticsReportResponse> OutbreakAnalyticsReport(OutbreakAnalyticsReportRequest request)
    {
        try{
            var query = _context.Outbreaks.AsQueryable();

            if (request.StartDate.HasValue)
            {
                query = query.Where(o => o.StartDate >= request.StartDate);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(o => o.EndDate <= request.EndDate);
            }
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                bool isActive = request.Status?.Equals("Active",StringComparison.OrdinalIgnoreCase) ?? false;
                bool isInActive = request.Status?.Equals("InActive",StringComparison.OrdinalIgnoreCase) ?? false;
                if (isActive)
                {
                    query = query.Where(o => o.Status == true);
                }
                else if (isInActive)
                {
                    query = query.Where(o => o.Status == false);
                }
            }
            if (!string.IsNullOrWhiteSpace(request.Region))
            {
                query = query.Where(o => o.Location == request.Region);
            }

            // Metrics
            var result = await query
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalOutbreaks = g.Count(),
                    ActiveOutbreaks = g.Count(x => x.Status == true),
                    ResolvedOutbreaks = g.Count(x => x.Status==false)
                }).FirstOrDefaultAsync();

            // Actual Data
             var data = await query.ToListAsync();

            return new OutbreakAnalyticsReportResponse
            {
                Success = true,
                TotalOutbreaks = result?.TotalOutbreaks ?? 0,
                ActiveOutbreaks = result?.ActiveOutbreaks ?? 0,
                ResolvedOutbreaks = result?.ResolvedOutbreaks ?? 0,
                GeneratedDate = DateTime.UtcNow,
                Data = data
            };
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while fetching the outbreaks reports "+ex.Message);
        }
    }
}
