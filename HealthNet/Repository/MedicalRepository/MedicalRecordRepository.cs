using System;
using HealthNetDb.Data;
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
}
