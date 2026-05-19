using System;
using HealthNet.DTOs.CaseDto;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.CaseRepository;

public class CasesRepository : ICasesRepository
{
    private readonly HealthNetContext _context;

    public CasesRepository(HealthNetContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new case in the database.
    /// </summary>
    public async Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId, int citizenId)
    {
        var cases = new HealthNetDb.Entities.Cases
        {
            CitizenId = citizenId,
            DoctorId  = doctorId,
            Diagnosis = request.Diagnosis,
            Status    = request.Status!.Value,
            Date      = DateTime.UtcNow
        };

        _context.Casess.Add(cases);
        await _context.SaveChangesAsync();

        return new CaseResponseDto { CaseId = cases.CaseId };
    }

    /// <summary>
    /// Gets all cases.
    /// </summary>
    public async Task<IEnumerable<CaseListDto>> GetAllCasesAsync()
    {
        return await _context.Casess
            .Select(c => new CaseListDto
            {
                CaseId    = c.CaseId,
                CitizenId = c.CitizenId,
                DoctorId  = c.DoctorId,
                Diagnosis = c.Diagnosis,
                Date      = c.Date,
                Status    = c.Status
            }).ToListAsync();
    }

    /// <summary>
    /// Gets a case by ID.
    /// </summary>
    public async Task<CaseListDto?> GetCaseByIdAsync(int caseId)
    {
        return await _context.Casess
            .Where(c => c.CaseId == caseId)
            .Select(c => new CaseListDto
            {
                CaseId    = c.CaseId,
                CitizenId = c.CitizenId,
                DoctorId  = c.DoctorId,
                Diagnosis = c.Diagnosis,
                Date      = c.Date,
                Status    = c.Status
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates only the diagnosis of a case.
    /// </summary>
    public async Task UpdateCaseDiagnosisAsync(int caseId, string diagnosis)
    {
        var cases = await _context.Casess.FirstOrDefaultAsync(c => c.CaseId == caseId);
        cases!.Diagnosis = diagnosis;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Sets Status = true (Recovered) then deletes the case.
    /// </summary>
    public async Task DeleteCaseAsync(int caseId)
    {
        var cases = await _context.Casess.FirstOrDefaultAsync(c => c.CaseId == caseId);
        
        //Set status to recovered first
        cases!.Status = true;
        await _context.SaveChangesAsync();
    }
}
