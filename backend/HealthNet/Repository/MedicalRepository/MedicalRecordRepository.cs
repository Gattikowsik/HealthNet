using System;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;
using HealthNetDb.Entities;

namespace HealthNet.Repository.MedicalRepository;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly HealthNetContext _context;

    public MedicalRecordRepository(HealthNetContext context)
    {
        _context = context;
    }

    public async Task<MedicalRecord> AddAsync(MedicalRecord record)
    {
        _context.MedicalRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<MedicalRecord?> GetLatestRecordByPatientIdAsync(int patientId)
    {
        return await _context.MedicalRecords
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<List<MedicalRecord>> GetRecordsByPatientIdAsync(int patientId)
    {
        return await _context.MedicalRecords
        .Include(m => m.Doctor)
        .Where(m => m.PatientId == patientId)
        .OrderByDescending(m => m.Date)
        .ToListAsync();
    }
}
