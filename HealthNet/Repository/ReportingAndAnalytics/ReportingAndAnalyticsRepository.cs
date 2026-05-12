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
        try
        {
            var query = _context.Outbreaks.Where(o => !o.IsDeleted).AsQueryable();
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
                bool isActive = request.Status?.Equals("Active", StringComparison.OrdinalIgnoreCase) ?? false;
                bool isInActive = request.Status?.Equals("InActive", StringComparison.OrdinalIgnoreCase) ?? false;
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
                    ResolvedOutbreaks = g.Count(x => x.Status == false)
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
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while fetching the outbreaks reports " + ex.Message);
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

            // Filter using Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (request.Status.ToLower().Equals("active"))
                {
                    query = query.Where(p => p.Status == PatientStatus.Active);
                }
                else if (request.Status.ToLower().Equals("inactive"))
                {
                    query = query.Where(p => p.Status == PatientStatus.InActive);
                }
            }
            //Filter using Date ranges for DOB
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                query = query.Where(p => request.StartDate <= p.DOB && p.DOB <= request.EndDate);
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
                ActivePatients = data.Count(x => x.Status == PatientStatus.Active),
                InActivePatients = data.Count(x => x.Status == PatientStatus.InActive),
                Data = data
            };
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while fetching the Patients records " + ex.Message);
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
                CompleteDocPercentage = totalRecords != 0 ? Math.Round((double)complaintRecords / totalRecords * 100, 2) : 0,
                Data = data
            };
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Compliance Metric records " + ex.Message);
        }
    }

    /// <summary>
    /// Get Epidemiology records from the Database
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  The Epidemiology records and metrics from the DB
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<EpidemiologicalAnalyticsReportResponse> EpidemiologyAnalyticsReport(EpidemiologicalAnalyticsReportRequest request)
    {
        try
        {
            var query = _context.Epidemiologies.Include(e => e.OutbreakNavigation).Where(e => !e.IsDeleted && !e.OutbreakNavigation.IsDeleted).AsQueryable();

            if (request.EpidemiologyDate.HasValue)
            {
                query = query.Where(ep => ep.Date == request.EpidemiologyDate || ep.Date.Date == request.EpidemiologyDate);
            }

            if (!string.IsNullOrWhiteSpace(request.OutbreakDisease))
            {
                query = query.Where(ep => ep.OutbreakNavigation.Disease.ToLower().Contains(request.OutbreakDisease.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.OutbreakLocation))
            {
                query = query.Where(ep => ep.OutbreakNavigation.Location.ToLower().Contains(request.OutbreakLocation));
            }

            if (request.OutbreakStartDate.HasValue)
            {
                query = query.Where(ep => request.OutbreakStartDate <= ep.OutbreakNavigation.StartDate);
            }

            if (request.OutbreakEndDate.HasValue)
            {
                query = query.Where(ep => ep.OutbreakNavigation.EndDate <= request.OutbreakEndDate);
            }

            if (!string.IsNullOrWhiteSpace(request.OutbreakStatus))
            {
                if (request.OutbreakStatus.ToLower().Equals("active"))
                {
                    query = query.Where(ep => ep.OutbreakNavigation.Status == true);
                }
                else if (request.OutbreakStatus.ToLower().Equals("inactive"))
                {
                    query = query.Where(ep => ep.OutbreakNavigation.Status == false);
                }
            }

            var result = await query.ToListAsync();
            List<EpidemiologyResponse> epidemiologyData = new List<EpidemiologyResponse>();

            foreach (var item in result)
            {
                EpidemiologyResponse epiResponse = new EpidemiologyResponse
                {
                    EpiId = item.EpiId,
                    MetricsJSON = item.MetricsJSON,
                    EpiDate = item.Date,
                    Disease = item.OutbreakNavigation.Disease,
                    Location = item.OutbreakNavigation.Location,
                    OutbreakStartDate = item.OutbreakNavigation.StartDate,
                    OutbreakEndDate = item.OutbreakNavigation.EndDate,
                    Severity = item.OutbreakNavigation.Severity
                };

                epidemiologyData.Add(epiResponse);
            }

            return new EpidemiologicalAnalyticsReportResponse
            {
                Success = true,
                TotalEpidemiologies = result.Count,
                ActiveEpidemiologies = result.Count(e => e.Status == true),
                ActiveOutbreaks = result.Count(e => e.OutbreakNavigation.Status == true),
                InActiveOutbreaks = result.Count(e => e.OutbreakNavigation.Status == false),
                EpidemiologyResponses = epidemiologyData
            };
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error Occurred while retrieving data from Epidemiology " + ex.Message);
        }
    }
}
