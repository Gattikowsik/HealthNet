using System;
using HealthNet.DTOs.OutbreakMonitoringDTO;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.OutbreakMonitoringRepository;

public class OutbreakMonitoringRepository : IOutBreakMonitoringRepository
{
    private readonly HealthNetContext _context;
    public OutbreakMonitoringRepository(HealthNetContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds Outbreak to the Database
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  The Outbreak Id after creation
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<int> AddOutbreakAsync(CreateOutbreakRequestDto request)
    {
        try
        {
            var outbreak = new Outbreak
            {
                Disease = request.Disease,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Severity = request.Severity,
                Status = request.Status
            };
            await _context.Outbreaks.AddAsync(outbreak);
            await _context.SaveChangesAsync();
            return outbreak.OutbreakId;
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error occured while adding the Outbreak " + ex.Message);
        }
    }

    /// <summary>
    /// Checks Whether the duplicate Outbreak Exists
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    ///  If Duplicate Exists returns true else false
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<bool> DuplicateOutbreakExitsAsync(CreateOutbreakRequestDto request)
    {
        try
        {
            return await _context.Outbreaks.AnyAsync(ob => ob.Disease == request.Disease && ob.Location == request.Location && ob.Status);
        }
        catch (Exception ex)
        {
            throw new HealthNetException("An Error Occured while Check for Duplicate Outbreak " + ex.Message);
        }
    }

    public async Task<int> AddAuditLogAsync(int userId, string actionName, string resource)
    {
        try
        {
            int actionId = await _context.Actions
                .Where(a => a.ActionName == actionName)
                .Select(a => a.ActionId)
                .FirstAsync();

            AuditLog auditLog = new AuditLog
            {
                UserId = userId,
                ActionId = actionId,
                Resource = resource,
                Timestamp = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();

            return auditLog.AuditId;
        }
        catch (Exception ex)
        {
            throw new HealthNetException(
                "An error occurred while logging audit: " + ex.Message);
        }
    }

    //GetOutbreakById
    public async Task<Outbreak?> GetOutbreakByIdAsync(int outbreakId)
    {
        try
        {
            return await _context.Outbreaks.AsNoTracking().FirstOrDefaultAsync(o => o.OutbreakId == outbreakId);
        }
        catch (Exception ex)
        {
            throw new HealthNetException("Error while fetching outbreak: " + ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing outbreak by modifying severity, end date, or status.
    /// The update is performed only if at least one field value is different
    /// from the existing data in the database.
    /// </summary>

    public async Task<UpdateOutbreakResult> UpdateOutbreakAsync(int outbreakId, UpdateOutbreakRequestDto request)
    {
        try
        {
            var outbreak = await _context.Outbreaks.FirstOrDefaultAsync(o => o.OutbreakId == outbreakId);
            if (outbreak == null)
                return UpdateOutbreakResult.NotFound;

            if (!outbreak.Status)
                return UpdateOutbreakResult.Closed;
            bool hasChanges = false;
            if (!string.Equals(outbreak.Severity, request.Severity, StringComparison.OrdinalIgnoreCase))
            {
                outbreak.Severity = request.Severity;
                hasChanges = true;
            }
            if (outbreak.EndDate != request.EndDate)
            {
                outbreak.EndDate = request.EndDate;
                hasChanges = true;
            }
            if (outbreak.Status != request.Status)
            {
                outbreak.Status = request.Status;
                hasChanges = true;
            }
            if (!hasChanges)
                return UpdateOutbreakResult.NoChanges;

            await _context.SaveChangesAsync();
            return UpdateOutbreakResult.Updated;
        }
        catch (Exception ex)
        {
            throw new HealthNetException("Error while updating outbreak: " + ex.Message);
        }
    }

    public async Task<bool> OutbreakExistsAsync(int outbreakId)
    {
        return await _context.Outbreaks.AnyAsync(o => o.OutbreakId == outbreakId);
    }

    public async Task<int> AddEpidemiologyAsync(Epidemiology epidemiology)
    {

        await _context.Epidemiologies.AddAsync(epidemiology);
        await _context.SaveChangesAsync();
        return epidemiology.EpiId;
    }
    public async Task<List<Outbreak>> GetAllActiveOutbreaksAsync()
    {
        try
        {
            return await _context.Outbreaks
                .AsNoTracking()
                .Where(o => o.Status == true) // ALL ACTIVE
                .OrderByDescending(o => o.StartDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new HealthNetException("Error while retrieving active outbreaks: " + ex.Message);
        }
    }
}
