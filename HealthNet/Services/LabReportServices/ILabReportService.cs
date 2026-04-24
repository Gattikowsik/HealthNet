using System;
using HealthNet.DTOs.LabReportDTO;

namespace HealthNet.Services.LabReportServices;

public interface ILabReportService
{
    Task<LabReportResponse> UploadLabReportAsync(LabReportRequest request, int userId, string webRootPath);
}
