using System;
using HealthNetDb.Entities;

namespace HealthNet.Repository.LabReportRepo;

public interface ILabReportRepository
{
    Task<LabTest?> GetLabTestByIdAsync(int testId);             // Get LabTest by TestId
    Task<bool> ReportExistsAsync(int testId);                   // Check duplicate report
    Task<LabReport> CreateLabReportAsync(LabReport labReport);  // Save LabReport
    Task UpdateLabTestStatusAsync(int testId);
    Task<IEnumerable<LabReport>> GetReportsByTestIdAsync(int testId);
}
