using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.LabReportRepo;

public class LabReportRepository : ILabReportRepository
{
    private readonly HealthNetContext _context;

    /// <summary>
    /// Constructor for LabReportRepository, injects HealthNetContext for database access.
    /// </summary>
    /// <param name="context">The database context used for accessing lab report data.</param>
    public LabReportRepository(HealthNetContext context)
    {
        _context = context;
    }
    public async Task<LabReport> CreateLabReportAsync(LabReport labReport)
    {
        try
        {
            await _context.LabReports.AddAsync(labReport);
            await _context.SaveChangesAsync();
            return labReport;
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            throw new HealthNetException($"An error occurred while saving lab report. {innerMessage}");
        }
    }

    public async Task<LabTest?> GetLabTestByIdAsync(int testId)
    {
        try
        {
            return await _context.LabTests
                .FirstOrDefaultAsync(lt => lt.TestId == testId);
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while fetching lab test. {ex.Message}");
        }
    }

    public async Task<bool> ReportExistsAsync(int testId)
    {
        try
        {
            return await _context.LabReports
                .AnyAsync(lr => lr.TestId == testId);
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while checking duplicate report. {ex.Message}");
        }
    }

    public async Task UpdateLabTestStatusAsync(int testId)
    {
        try
        {
            var labTest = await _context.LabTests
                .FirstOrDefaultAsync(lt => lt.TestId == testId);

            if (labTest != null)
            {
                labTest.Status = true;  // Completed
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while updating lab test status. {ex.Message}");
        }
    }

    // Fetches all lab reports associated with a specific lab test.
    public async Task<IEnumerable<LabReport>> GetReportsByTestIdAsync(int testId)
    {
        try
        {
            return await _context.LabReports
                .Where(lr => lr.TestId == testId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while fetching lab reports. {ex.Message}");
        }
    }
}
