using System;
using HealthNet.Repository.MedicalRepository;
using Microsoft.EntityFrameworkCore;
using HealthNet.DTOs.MedicalRecordDto;
using HealthNetDb.Data;
using HealthNetDb.Entities;

namespace HealthNet.Services.MedicalServices;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly HealthNetContext _context;
    private readonly IMedicalRecordRepository _repository;

    public MedicalRecordService(HealthNetContext context, IMedicalRecordRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public async Task<MedicalRecordResponseDto> AddMedicalRecordAsync(int patientId, int doctorId, MedicalRecordRequestDto dto)
    {
        if (dto.Date > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return new MedicalRecordResponseDto
            {
                Success = false,
                Message = "Date cannot be in the future."
            };
        }
        bool patientExists = await _context.Patients
            .AnyAsync(p => p.PatientId == patientId);

        if (!patientExists)
            throw new KeyNotFoundException("Patient not found");

        var record = new MedicalRecord
        {
            PatientId = patientId,
            DoctorId = doctorId,
            Diagnosis = dto.Diagnosis,
            TreatmentPlan = dto.TreatmentPlan,
            Date = dto.Date,
            Status = dto.Status
        };

        var saved = await _repository.AddAsync(record);


        var actionId = await _context
        .Set<HealthNetDb.Entities.Action>()
        .Where(a => a.ActionName == "Create")
        .Select(a => a.ActionId)
        .FirstAsync();

        var auditLog = new AuditLog
        {
            UserId = doctorId,
            ActionId = actionId,
            Resource = "MedicalRecord",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();


        return new MedicalRecordResponseDto
        {
            Success = true,
            RecordId = saved.RecordId
        };
    }
}
