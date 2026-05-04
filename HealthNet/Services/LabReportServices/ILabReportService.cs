using System;
using HealthNet.DTOs.LabReportDTO;

namespace HealthNet.Services.LabReportServices;

public interface ILabReportService
{
    Task<LabReportResponse> UploadLabReportAsync(LabReportRequest request, int userId);
    Task<LabTestWithReportsResponse> GetReportsByTestIdAsync(int testId, int userId);
    Task<(byte[] FileData, string FileName, string ContentType)> DownloadReportAsync(int testId, int userId);
}
