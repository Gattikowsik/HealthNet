using System;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;
using HealthNet.Repository.ReportingAndAnalytics;
using HealthNet.Utility;
using HealthNetDb.Entities;

namespace HealthNet.Services.ReportingAndAnalyticsServices;

public class ReportingAndAnalyticsService : IReportingAndAnalyticsService
{
    private readonly IReportingAndAnalyticsRepository _repository;
    public ReportingAndAnalyticsService(IReportingAndAnalyticsRepository repository)
    {
        _repository = repository;
    }

    // Get Outbreaks Service
    /// <summary>
    /// The request will be validated and sent to the repository.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// DTO which consist of Message,and Outbreaks else Error Message
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<OutbreakAnalyticsReportResponse> OutbreakAnalyticsReportService(OutbreakAnalyticsReportRequest request)
    {
        try
        {
            // Validate the Date fields
            var presentDateTime = DateTime.UtcNow;
            if (request.StartDate >= presentDateTime)
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date cannot be in Future."
                };
            }
            if (request.EndDate >= presentDateTime)
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "End Date Cannot be in Future."
                };
            }
            if (request.StartDate >= request.EndDate)
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date must be earlier than the End Date."
                };
            }

            // Validate the Status Field
            if (!string.IsNullOrWhiteSpace(request.Status) && !string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.Status, "InActive", StringComparison.OrdinalIgnoreCase))
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Status must be either Active or InActive."
                };
            }

            return await _repository.OutbreakAnalyticsReport(request);
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Outbreaks data" + ex.Message);
        }
    }

    // Get Patients Service
    /// <summary>
    /// The request will be validated and sent to the repository.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// DTO which consist of Message,and Patients else Error Message
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<PatientAnalyticsReportResponse> PatientAnalyticsReportService(PatientAnalyticsReportRequest request)
    {
        try
        {
            // Validate the Age
            if (request.MinAge.HasValue && (request.MinAge <= 0 || request.MinAge > 200))
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Minimum Age Cannot be less than 1 and greater than 200."
                };
            }
            if (request.MaxAge.HasValue && (request.MaxAge > 200 || request.MaxAge <= 0))
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Maximum Age Cannot be greater than 200 and less than 1."
                };
            }
            if (request.MinAge.HasValue && request.MaxAge.HasValue && request.MaxAge < request.MinAge)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Minimum Age Cannot be greater than Maximum Age."
                };
            }

            // Validate Gender Field
            if (!string.IsNullOrWhiteSpace(request.Gender)
                && !request.Gender.ToLower().Equals("male")
                && !request.Gender.ToLower().Equals("female")
                && !request.Gender.ToLower().Equals("other")
            )
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Gender Must be one of these Male, Female, Other."
                };
            }

            // Validate Status Field
            if(!string.IsNullOrWhiteSpace(request.Status) 
                && !request.Status.ToLower().Equals("active")
                && !request.Status.ToLower().Equals("inactive"))
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Status must be either active or inactive."
                };
            }

            // Validate Date Fields
            DateOnly presentDateTime = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.StartDate >= request.EndDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date Cannot be earlier than End Date."
                };
            }
            if (presentDateTime < request.StartDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date Cannot be in Future."
                };
            }
            if (presentDateTime < request.EndDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "End Date Cannot be in Future."
                };
            }

            return await _repository.PatientAnalyticsReport(request);
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Patients data " + ex.InnerException);
        }
    }

    // Get Compliance Records and Metrics Service
    /// <summary>
    /// The request will be validated and sent to the repository.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// DTO which consist of Message,and Compliance Records else Error Message
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<ComplianceMetricsReportResponse> ComplianceMetricsReportService(ComplianceMetricsReportRequest request)
    {
        try
        {
            // Validate Date field
            var presentDateTime = DateTime.UtcNow;
            if (presentDateTime < request.DateFilter)
            {
                return new ComplianceMetricsReportResponse
                {
                    Success = false,
                    Message = "Date Cannot be in Future."
                };
            }

            // Validate Type field
            var allowedTypes = new[] { "case", "test", "outbreak" };
            if (!string.IsNullOrWhiteSpace(request.TypeFilter) && !allowedTypes.Contains(request.TypeFilter.ToLower()))
            {
                return new ComplianceMetricsReportResponse
                {
                    Success = false,
                    Message = "Type Must be one of these case, test, outbreak"
                };
            }

            // Validate Result field
            var allowedResults = new[] { "compliant", "non compliant", "partially compliant", "pending review" };
            if (!string.IsNullOrWhiteSpace(request.ResultFilter) && !allowedResults.Contains(request.ResultFilter.ToLower()))
            {
                return new ComplianceMetricsReportResponse
                {
                    Success = false,
                    Message = "Result must be one of these types compliant non compliant partially compliant pending review"
                };
            }

            return await _repository.ComplianceMetricsReport(request);
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving Compliance records data " + ex.Message);
        }
    }

    // Get Epidemiology Records and Metrics Service
    /// <summary>
    /// The request will be validated and sent to the repository.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// DTO which consist of Message,and Epidemiology and Outbreak Records else Error Message
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<EpidemiologicalAnalyticsReportResponse> EpidemiologicalReportService(EpidemiologicalAnalyticsReportRequest request)
    {
        try
        {
            // Validate the Epidemiology Date
            var presentDate = DateTime.UtcNow;
            if (presentDate < request.EpidemiologyDate)
            {
                return new EpidemiologicalAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Date cannot be in Future."
                };
            }

            // Validate the Outbreak Start Date
            if (presentDate < request.OutbreakStartDate)
            {
                return new EpidemiologicalAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Outbreak Start Date cannot be in Future."
                };
            }
            // Validate the Outbreak End Date
            if (presentDate < request.OutbreakEndDate)
            {
                return new EpidemiologicalAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Outbreak End Date cannot be in Future."
                };
            }
            if (request.OutbreakEndDate < request.OutbreakStartDate)
            {
                return new EpidemiologicalAnalyticsReportResponse
                {
                    Success = false,
                    Message = "End Date Cannot be earlier than Start Date"
                };
            }

            // Validate Outbreak Status
            if (!string.IsNullOrWhiteSpace(request.OutbreakStatus)
                && !request.OutbreakStatus.ToLower().Equals("active")
                && !request.OutbreakStatus.ToLower().Equals("inactive"))
            {
                return new EpidemiologicalAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Outbreak Status must be in either active or inactive state."
                };
            }

            return await _repository.EpidemiologyAnalyticsReport(request);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException("An Error Occurred while retrieving the data from Repository " + ex.Message);
        }
    }
}