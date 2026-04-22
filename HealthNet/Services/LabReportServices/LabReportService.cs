using HealthNet.DTOs.LabReportDTO;
using HealthNet.Repository.LabReportRepo;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Services.LabReportServices;

public class LabReportService : ILabReportService
{
    private readonly ILabReportRepository _labReportRepository;
    private readonly HealthNetContext _context;

    /// <summary>
    /// Constructor for LabReportService, injects ILabReportRepository and HealthNetContext.
    /// </summary>
    /// <param name="labReportRepository">The lab report repository used to access lab report data.</param>
    /// <param name="context">The database context used for audit logging.</param>
    public LabReportService(ILabReportRepository labReportRepository, HealthNetContext context)
    {
        _labReportRepository = labReportRepository;
        _context             = context;
    }

    // Uploads a lab report for a completed lab test.
    public async Task<LabReportResponse> UploadLabReportAsync(LabReportRequest request, int userId)
    {
        try
        {
            // Validate FileURI is a valid URL
            if (!LabReportHelper.IsValidFileUri(request.FileURI))
            {
                throw new HealthNetException(LabReportHelper.InvalidUrlMessage);
            }

            // Validate TestId exists
            var labTest = await _labReportRepository.GetLabTestByIdAsync(request.TestId);
            if (labTest == null)
            {
                throw new HealthNetException(LabReportHelper.TestNotFoundMessage);
            }

            // Validate LabTest is Pending (Status = false)
            if (labTest.Status == true)
            {
                throw new HealthNetException(LabReportHelper.TestNotPendingMessage);
            }

            // Validate only assigned technician can upload
            if (labTest.TechnicianId != userId)
            {
                throw new HealthNetException(LabReportHelper.UnauthorizedTechnicianMessage);
            }

            // Validate duplicate report
            bool reportExists = await _labReportRepository.ReportExistsAsync(request.TestId);
            if (reportExists)
            {
                throw new HealthNetException(LabReportHelper.DuplicateReportMessage);
            }

            // Generate SHA256 hash of FileURI
            string fileHash = LabReportHelper.GenerateFileHash(request.FileURI);

            //  Map request to LabReport entity
            var labReport = new LabReport
            {
                TestId   = request.TestId,
                FileURI  = request.FileURI,
                FileHash = fileHash,
                Date     = DateTime.UtcNow,
                Status   = false    // Not Verified by default
            };

            // Save LabReport to DB
            var created = await _labReportRepository.CreateLabReportAsync(labReport);

            // Update LabTest Status to Completed
            await _labReportRepository.UpdateLabTestStatusAsync(request.TestId);

            // Fetch ActionId for "Create"
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // Save AuditLog
            var auditLog = new AuditLog
            {
                UserId    = userId,
                ActionId  = actionId,
                Resource  = "Lab Report",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // Map entity to response DTO
            return new LabReportResponse
            {
                ReportId = created.ReportId,
                TestId   = created.TestId,
                FileURI  = created.FileURI,
                FileHash = created.FileHash,
                Date     = created.Date,
                Status   = created.Status
            };
        }
        catch (HealthNetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while uploading lab report. {ex.Message}");
        }
    }
}