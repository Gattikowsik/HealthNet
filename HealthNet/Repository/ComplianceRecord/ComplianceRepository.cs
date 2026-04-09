using System;
using HealthNet.DTOs.ComplianceRecordDto;
using HealthNetDb.Data;
using HealthNetDb.Entities;
namespace HealthNet.Repository.ComplianceRecord;

public class ComplianceRepository : IComplianceRepository
{
    HealthNetContext _context;
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
            Date = request.Date,
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
}