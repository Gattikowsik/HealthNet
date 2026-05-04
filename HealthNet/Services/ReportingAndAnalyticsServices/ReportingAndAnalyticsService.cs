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
            if(request.StartDate >= presentDateTime)
            {
                return new  OutbreakAnalyticsReportResponse{
                    Success = false,
                    Message = "Start Date cannot be in Future."
                };
            }
            if(request.EndDate >= presentDateTime)
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "End Date Cannot be in Future."
                };
            }
            if(request.StartDate >= request.EndDate)
            {
                return new OutbreakAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date must be earlier than the End Date."
                };
            }

            // Validate the Status Field
            if(!string.IsNullOrWhiteSpace(request.Status) && !string.Equals(request.Status,"Active",StringComparison.OrdinalIgnoreCase) &&
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
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Outbreaks data"+ex.Message);
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
            if(request.MinAge.HasValue && (request.MinAge<=0 || request.MinAge>150))
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Minimum Age Cannot be less than 1 and greater than 150."
                };
            }
            if(request.MaxAge.HasValue && (request.MaxAge > 150 || request.MaxAge<=0))
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Maximum Age Cannot be greater than 150 and less than 1."
                };
            }
            if(request.MinAge.HasValue && request.MaxAge.HasValue && request.MaxAge < request.MinAge)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Minimum Age Cannot be greater than Maximum Age."
                };
            }
            
            // Validate Gender Field
            if(!string.IsNullOrWhiteSpace(request.Gender) 
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
            
            // Validate Date Fields
            DateOnly presentDateTime = DateOnly.FromDateTime(DateTime.UtcNow);
            if(request.StartDate >= request.EndDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date Cannot be earlier than End Date."
                };
            }
            if(presentDateTime < request.StartDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "Start Date Cannot be in Future."
                };
            }
            if(presentDateTime < request.EndDate)
            {
                return new PatientAnalyticsReportResponse
                {
                    Success = false,
                    Message = "End Date Cannot be in Future."
                };
            }
            

            return await _repository.PatientAnalyticsReport(request); 
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while retrieving the Patients data "+ex.InnerException);
        }
    }
}