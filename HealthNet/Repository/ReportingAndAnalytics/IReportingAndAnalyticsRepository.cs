using System;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;

namespace HealthNet.Repository.ReportingAndAnalytics;

public interface IReportingAndAnalyticsRepository
{
    Task<OutbreakAnalyticsReportResponse> OutbreakAnalyticsReport(OutbreakAnalyticsReportRequest request);
}
