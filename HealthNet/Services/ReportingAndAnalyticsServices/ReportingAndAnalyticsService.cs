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

    // Get Outbreak Service
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
            throw new HealthNetException("An Error occured while retrieving date from Outbreaks "+ex.Message);
        }
    }
}
