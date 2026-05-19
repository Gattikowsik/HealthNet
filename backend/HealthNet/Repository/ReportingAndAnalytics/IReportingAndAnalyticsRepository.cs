using HealthNet.DTOs.ReportingAndAnalyticsDTO;

namespace HealthNet.Repository.ReportingAndAnalytics;

public interface IReportingAndAnalyticsRepository
{
    Task<OutbreakAnalyticsReportResponse> OutbreakAnalyticsReport(OutbreakAnalyticsReportRequest request);
    Task<PatientAnalyticsReportResponse> PatientAnalyticsReport(PatientAnalyticsReportRequest request);
    Task<ComplianceMetricsReportResponse> ComplianceMetricsReport(ComplianceMetricsReportRequest request);
    Task<EpidemiologicalAnalyticsReportResponse> EpidemiologyAnalyticsReport(EpidemiologicalAnalyticsReportRequest request);
    Task<CaseAnalyticsReportResponse> CaseAnalyticsReport(CaseAnalyticsReportRequest request);
}