using System;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using HealthNetDb.Enums;
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

    /// <summary>
    /// Get Patients Data from the Database
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  The Patients Data from the DB
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<PatientAnalyticsReportResponse> PatientAnalyticsReport(PatientAnalyticsReportRequest request)
    {
        try
        {
            var query = _context.Patients.AsQueryable();

            // Filter using Age
            if (request.MinAge.HasValue)
            {
                query = query.Where(p => request.MinAge <= (DateTime.UtcNow.Year - p.DOB.Year));
            }
            if (request.MaxAge.HasValue)
            {
                query = query.Where(p => (DateTime.UtcNow.Year - p.DOB.Year) <= request.MaxAge);
            }

            // Filter using Gender
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                query = query.Where(p => p.Gender.ToLower().Equals(request.Gender.ToLower()));
            }

            //Filter using Date ranges for DOB
            if(request.StartDate.HasValue && request.EndDate.HasValue)
            {
                query = query.Where(p => request.StartDate<=p.DOB && p.DOB<=request.EndDate);
            }
            else if (request.StartDate.HasValue)
            {
                query = query.Where(p => request.StartDate <= p.DOB);
            }
            else if (request.EndDate.HasValue)
            {
                query = query.Where(p => p.DOB <= request.EndDate);
            }
            
            
            var data = await query.ToListAsync();    
            
            return new PatientAnalyticsReportResponse
            {
                Success = true,
                TotalPatients = data.Count,
                RegisteredPatients = data.Count(x => x.Status == PatientStatus.Registered),
                UnderTreatmentPatients = data.Count(x => x.Status == PatientStatus.UnderTreatment),
                RecoveredPatients = data.Count(x => x.Status == PatientStatus.Recovered),
                DischargedPatients = data.Count(x => x.Status == PatientStatus.Discharged),
                Data = data
            };
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while fetching the Patients records "+ex.Message);
        }
    }

    /// <summary>
    /// Get Compliance records from the Database
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  The Compliance records and metrics from the DB
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<ComplianceMetricsReportResponse> ComplianceMetricsReport(ComplianceMetricsReportRequest request)
    {
        try
        {
            var query = _context.ComplianceRecords.AsQueryable();

            if (request.DateFilter.HasValue)
            {
                query = query.Where(cr => cr.Date.Date == request.DateFilter || cr.Date == request.DateFilter);
            }
            if (!string.IsNullOrWhiteSpace(request.TypeFilter))
            {
                query = query.Where(cr => cr.Type != null && cr.Type.ToLower().Equals(request.TypeFilter.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(request.ResultFilter))
            {
                query = query.Where(cr => cr.Result.ToLower().Equals(request.ResultFilter.ToLower()));
            }

            var data = await query.ToListAsync();
            int complaintRecords = data.Count(x => x.Result.ToLower().Equals(ComplianceRecordResultHelper.Compliant));
            int totalRecords = data.Count;
            return new ComplianceMetricsReportResponse
            {
                Success = true,
                TotalCompliances = totalRecords,
                CompliantRecords = complaintRecords,
                NonCompliantRecords = data.Count(x => x.Result.ToLower().Equals(ComplianceRecordResultHelper.NonComplaint)),
                PartiallyCompliantRecords = data.Count(x => x.Result.ToLower().Equals(ComplianceRecordResultHelper.PartiallyCompliant)),
                PendingReviewRecords = data.Count(x => x.Result.ToLower().Equals(ComplianceRecordResultHelper.PendingReview)),
                CompleteDocPercentage = totalRecords!=0 ? Math.Round((double)complaintRecords/totalRecords*100,2) : 0,
                Data = data
            };
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Compliance Metric records "+ex.Message);
        }
    }
}
