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
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while adding the Outbreak "+ex.Message);
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
            return await _context.Outbreaks.AnyAsync(ob => ob.Disease==request.Disease && ob.Location==request.Location && ob.Status);
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error Occured while Check for Duplicate Outbreak "+ex.Message);
        }
    }

    public async Task<int> AddAuditLogAsync(int userId, string resource)
    {
        try
        {
            int actionId = await (from a in _context.Actions where a.ActionName=="Create" select a.ActionId).FirstAsync();
            AuditLog auditLog = new AuditLog()
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
        catch(Exception ex)
        {
            throw new HealthNetException("An Error occured while Logging Outbreak to Audit. "+ex.Message);
        }
    }
}
