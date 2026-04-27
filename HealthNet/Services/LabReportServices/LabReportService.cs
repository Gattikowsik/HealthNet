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
    public async Task<LabReportResponse> UploadLabReportAsync(LabReportRequest request, int userId, string webRootPath)
    {
        try
        {
            // Validate FileURI is a valid URL
            if (!LabReportHelper.IsValidFile(request.File))
            {
                throw new HealthNetException(LabReportHelper.InvalidFileMessage);
            }

            // Validate file size (max 10 MB)
            if (request.File.Length > 10 * 1024 * 1024)
            {
                throw new HealthNetException(LabReportHelper.FileTooLargeMessage);
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
                throw new UnauthorizedAccessException(LabReportHelper.UnauthorizedTechnicianMessage);
            }

            // Validate duplicate report
            bool reportExists = await _labReportRepository.ReportExistsAsync(request.TestId);
            if (reportExists)
            {
                throw new HealthNetException(LabReportHelper.DuplicateReportMessage);
            }

            // Generate SHA256 hash of FileURI
            string fileHash = await LabReportHelper.GenerateFileHashAsync(request.File);

            //Logic to save file to local storage and get URI
            // Create reports folder if it doesn't exist
            var reportsFolder = Path.Combine(webRootPath, "reports");
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }

            // Generate unique filename — testId + timestamp + original extension
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            var fileName  = $"labtest_{request.TestId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath  = Path.Combine(reportsFolder, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }
            // Return relative URI
            string fileUri = $"/reports/{fileName}";

            //  Map request to LabReport entity
            var labReport = new LabReport
            {
                TestId   = request.TestId,
                FileURI  = fileUri,
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
        catch (UnauthorizedAccessException)
        {
            throw;
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

    // Gets all reports for a specific lab test.
    public async Task<LabTestWithReportsResponse> GetReportsByTestIdAsync(int testId, int userId)
    {
        try
        {
            // Validate TestId exists
            var labTest = await _labReportRepository.GetLabTestByIdAsync(testId);
            if (labTest == null)
            {
                return null!;   // null signals 404 to controller
            }

            // Get all reports for this test
            var reports = await _labReportRepository.GetReportsByTestIdAsync(testId);

            // Check if any reports exist
            if (!reports.Any())
            {
                return null!;   // null signals 404 to controller
            }

            // Fetch ActionId for "Read"
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Read")
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

            // Map entities to response DTO
            return new LabTestWithReportsResponse
            {
                TestId       = labTest.TestId,
                PatientId    = labTest.PatientId,
                Type         = labTest.Type,
                Date         = labTest.Date,
                TechnicianId = labTest.TechnicianId,
                TestStatus   = labTest.Status,
                Reports      = reports.Select(r => new LabReportSummaryResponse
                {
                    ReportId = r.ReportId,
                    FileURI  = r.FileURI,
                    Date     = r.Date,
                    Status   = r.Status
                })
            };
        }
        catch (HealthNetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while fetching lab reports. {ex.Message}");
        }
    }
}