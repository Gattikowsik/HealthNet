using HealthNet.DTOs.ReportingAndAnalyticsDTO;

namespace HealthNet.Services.ReportingAndAnalyticsServices;

public interface IReportingAndAnalyticsService
{
    Task<OutbreakAnalyticsReportResponse> OutbreakAnalyticsReportService(OutbreakAnalyticsReportRequest request);
    Task<PatientAnalyticsReportResponse> PatientAnalyticsReportService(PatientAnalyticsReportRequest request);
}
