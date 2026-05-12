using System;
using HealthNet.DTOs.ComplianceRecordDto;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNet.Repository.ComplianceRecord;

public class ComplianceRepository : IComplianceRepository
{
    private readonly HealthNetContext _context;
    public ComplianceRepository(HealthNetContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Creates a new compliance record in the database.
    /// </summary>
    /// <param name="request">The DTO containing the compliance record data.</param>
    /// <returns>The response DTO containing the created compliance record.</returns>
    public async Task<ComplianceRecordResponseDto> CreateComplianceRecordAsync(CreateComplianceRecordDto request)
    {
        // Map DTO → Entity here inside the repository 
        var record = new HealthNetDb.Entities.ComplianceRecord
        {
            EntityId = request.EntityId,
            Type = request.Type.ToLower(),
            Result = request.Result,
            Date = DateTime.UtcNow,
            Notes = request.Notes
        };

        // Save to DB — EF Core generates the ComplianceId after this
        _context.ComplianceRecords.Add(record);
        await _context.SaveChangesAsync();

        // Return the response DTO with the generated ID
        return new ComplianceRecordResponseDto
        {
            ComplianceId = record.ComplianceId
        };
    }

    /// <summary>
    /// Returns a filtered list of compliance records.
    /// </summary>
    /// <param name="filter">The DTO containing optional filter parameters.</param>
    /// <returns>List of ComplianceRecordListDto matching the filters.</returns>
    public async Task<IEnumerable<ComplianceRecordListDto>> GetComplianceRecordsAsync(ComplianceRecordFilterDto filter)
    {
        // Start with all records
        // IQueryable means nothing hits the DB yet — just building the query
        var query = _context.ComplianceRecords.AsQueryable();

        // Apply each filter only if it was provided by the user
        if (filter.EntityId.HasValue)
            query = query.Where(c => c.EntityId == filter.EntityId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Type))
            query = query.Where(c => c.Type == filter.Type.ToLower());

        if (!string.IsNullOrWhiteSpace(filter.Result))
            query = query.Where(c => c.Result == filter.Result.ToLower());

        if (filter.Date.HasValue)
            query = query.Where(c => c.Date.Date == filter.Date.Value.Date);

        // THIS is where the actual SQL hits the DB
        // Map each entity to the list DTO
        return await query.Select(c => new ComplianceRecordListDto
        {
            ComplianceId = c.ComplianceId,
            EntityId = c.EntityId,
            Type = c.Type,
            Result = c.Result,
            Date = c.Date,
            Notes = c.Notes
        }).ToListAsync();
    }

    /// <summary>
    /// Updates the Result and Notes of an existing compliance record.
    /// </summary>
    /// <param name="complianceId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task UpdateComplianceRecordAsync(int complianceId, UpdateComplianceRecordDto request)
    {
        var record = await _context.ComplianceRecords
            .FirstOrDefaultAsync(c => c.ComplianceId == complianceId);

        record!.Result = request.Result.ToLower();
        record!.Notes = request.Notes;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a compliance record by setting IsDeleted to true.
    /// </summary>
    /// <param name="complianceId"></param>
    /// <returns></returns>
    public async Task DeleteComplianceRecordAsync(int complianceId)
    {
        var record = await _context.ComplianceRecords
            .FirstOrDefaultAsync(c => c.ComplianceId == complianceId);

        record!.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Returns a single compliance record by its ID.
    /// </summary>
    /// <param name="complianceId">The ID of the compliance record</param>
    /// <returns>ComplianceRecordListDto matching the given ID, or null if not found</returns>
    public async Task<ComplianceRecordListDto?> GetComplianceRecordByIdAsync(int complianceId)
    {
        return await _context.ComplianceRecords
            .Where(c => c.ComplianceId == complianceId)
            .Select(c => new ComplianceRecordListDto
            {
                ComplianceId = c.ComplianceId,
                EntityId = c.EntityId,
                Type = c.Type,
                Result = c.Result,
                Date = c.Date,
                Notes = c.Notes
            })
            .FirstOrDefaultAsync();
    }
}