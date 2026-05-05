using System;
using HealthNet.Repository.MedicalRepository;
using Microsoft.EntityFrameworkCore;
using HealthNet.DTOs.MedicalRecordDto;
using System.Security.Claims;
using HealthNetDb.Data;
using HealthNetDb.Enums;
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
        {
            return new MedicalRecordResponseDto
            {
                Success = false,
                Message = "Patient not found"
            };

        }
        var lastRecord = await _repository.GetLatestRecordByPatientIdAsync(patientId);

        if (lastRecord != null &&
            lastRecord.Status != PatientStatus.InActive)
        {
            return new MedicalRecordResponseDto
            {
                Success = false,
                Message =
                    "A medical record is already active for this patient. Please close the current record before adding a new one."
            };
        }

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
    public async Task<Dictionary<DateOnly, List<MedicalRecordGetDto>>> GetPatientRecordsAsync(int patientId,int userId)
    {

        bool patientExists = await _context.Patients
              .AnyAsync(p => p.PatientId == patientId);

        if (!patientExists)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        var records = await _repository.GetRecordsByPatientIdAsync(patientId);
        await AddAuditLog(userId, "Read");
        return records
            .OrderByDescending(r => r.Date)
            .GroupBy(r => r.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Select(MapFull).ToList()
            );
    }

    private static MedicalRecordGetDto MapFull(MedicalRecord record)
    {
        return new MedicalRecordGetDto
        {
            Date = record.Date,
            Diagnosis = record.Diagnosis,
            TreatmentPlan = record.TreatmentPlan,
            Status = record.Status.ToString()
        };
    }

    private async Task AddAuditLog(int userId, string action)
    {
        var actionId = await _context.Actions
            .Where(a => a.ActionName == action)
            .Select(a => a.ActionId)
            .FirstAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionId = actionId,
            Resource = "MedicalRecord",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}
