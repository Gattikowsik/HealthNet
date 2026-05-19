using System;
using HealthNet.DTOs.PatientDto;
using System.Text.RegularExpressions;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;
using HealthNetDb.Data;
using HealthNetDb.Enums;
using Microsoft.EntityFrameworkCore;
using HealthNet.Repository.PatientRepository;

namespace HealthNet.Services.PatientServices;

public class PatientManagementService : IPatientManagementService
{
    private readonly IPatientManagementRepository _patientRepository;
    private readonly HealthNetContext _context;

    public PatientManagementService(IPatientManagementRepository patientRepository, HealthNetContext context)
    {
        _patientRepository = patientRepository;
        _context = context;
    }

    public async Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto, int userId)
    {

        if (!string.IsNullOrWhiteSpace(searchDto.Name) &&
                !Regex.IsMatch(searchDto.Name, @"^[A-Za-z\s]+$"))
        {
            throw new ArgumentException("Name must contain only alphabets and spaces.");
        }

        var result = await _patientRepository.SearchPatientsAsync(searchDto);

        var actionId = await _context
            .Set<HealthNetDb.Entities.Action>()
            .Where(a => a.ActionName == "Read")
            .Select(a => a.ActionId)
            .FirstAsync();

        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = actionId,
            Resource = "Patient",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto dto, int userId)
    {
        // ✅ 1. Name validation (alphabets and spaces only)
        if (!Regex.IsMatch(dto.Name, @"^[A-Za-z\s]+$"))
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "Name must contain only alphabets and spaces."
            };
        }

        // ✅ 2. DOB validation (no future dates)
        if (dto.DOB > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "Date of birth cannot be in the future."
            };
        }

        // ✅ 3. Gender validation
        var allowedGenders = new[] { "Male", "Female", "Other" };
        if (!allowedGenders.Contains(dto.Gender, StringComparer.OrdinalIgnoreCase))
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "Gender must be Male, Female, or Other."
            };
        }

        // ✅ 4. ContactInfo validation (digits only)
        if (!Regex.IsMatch(dto.ContactInfo, @"^\d+$"))
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "Contact number must contain digits only."
            };
        }

        bool activePatientExists = await _context.Patients.AnyAsync(p =>
               p.Name == dto.Name &&
               p.ContactInfo == dto.ContactInfo &&
               p.Status == PatientStatus.Active
           );

        if (activePatientExists)
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "Patient is already active. Please discharge the patient before re-registration."
            };
        }

        var patient = new Patient
        {
            Name = dto.Name,
            DOB = dto.DOB,
            Gender = dto.Gender,
            Address = dto.Address,
            ContactInfo = dto.ContactInfo,
            Status = PatientStatus.Active
        };

        var saved = await _patientRepository.AddAsync(patient);
        var actionId = await _context
            .Set<HealthNetDb.Entities.Action>()
            .Where(a => a.ActionName == "Create")
            .Select(a => a.ActionId)
            .FirstAsync();

        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionId = actionId,
            Resource = "Patient",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        return new RegisterPatientResponseDto
        {
            Success = true,
            PatientId = saved.PatientId
        };
    }

    public async Task<DeactivatePatientDto> DeactivatePatientAsync(int patientId, int userId)
{
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.PatientId == patientId);

    // ✅ 1. Patient not found
    if (patient == null)
    {
        return new DeactivatePatientDto
        {
            Success = false,
            Message = "Patient not found"
        };
    }

    // ✅ 2. Check if already inactive
    if (patient.Status == PatientStatus.InActive)
    {
        return new DeactivatePatientDto
        {
            Success = false,
            Message = "Patient is already inactive"
        };
    }

    // ✅ 3. Check active medical records FIRST (VERY IMPORTANT)
    var activeRecordExists = await _context.MedicalRecords
        .AnyAsync(r => r.PatientId == patientId &&
                       r.Status == MedicalRecordStatus.Active);

    if (activeRecordExists)
    {
        return new DeactivatePatientDto
        {
            Success = false,
            Message = "Cannot deactivate patient. Please close all active medical records first."
        };
    }

    // ✅ 4. Safe to deactivate
    patient.Status = PatientStatus.InActive;

    await _context.SaveChangesAsync();

    // ✅ 5. Audit log
    var actionId = await _context.Actions
        .Where(a => a.ActionName == "Delete")
        .Select(a => a.ActionId)
        .FirstAsync();

    _context.AuditLogs.Add(new AuditLog
    {
        UserId = userId,
        ActionId = actionId,
        Resource = "Patient",
        Timestamp = DateTime.UtcNow
    });

    await _context.SaveChangesAsync();

    // ✅ 6. Success response
    return new DeactivatePatientDto
    {
        Success = true,
        Message = "Patient deactivated successfully"
    };
}
    public async Task<Patient?> GetPatientByIdAsync(int patientId, int userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (patient == null)
            return null;

        var actionId = await _context.Actions
            .Where(a => a.ActionName == "Read")
            .Select(a => a.ActionId)
            .FirstAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionId = actionId,
            Resource = "Patient",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return patient;
    }
    public async Task<UpdatePatientResponseDto> UpdatePatientAsync(int patientId, UpdatePatientDto dto,
    int userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (patient == null)
            throw new KeyNotFoundException();

        if (!Regex.IsMatch(dto.Name, @"^[A-Za-z\s]+$"))
        {
            return new UpdatePatientResponseDto
            {
                Success = false,
                Message = "Name must contain only alphabets."
            };
        }

        if (dto.DOB > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return new UpdatePatientResponseDto
            {
                Success = false,
                Message = "DOB cannot be in future."
            };
        }

        var allowedGenders = new[] { "Male", "Female", "Other" };
        if (!allowedGenders.Contains(dto.Gender, StringComparer.OrdinalIgnoreCase))
        {
            return new UpdatePatientResponseDto
            {
                Success = false,
                Message = "Invalid gender."
            };
        }

        if (!Regex.IsMatch(dto.ContactInfo, @"^\d+$"))
        {
            return new UpdatePatientResponseDto
            {
                Success = false,
                Message = "Contact must be digits only."
            };
        }

        // Update fields
        patient.Name = dto.Name;
        patient.DOB = dto.DOB;
        patient.Gender = dto.Gender;
        patient.Address = dto.Address;
        patient.ContactInfo = dto.ContactInfo;

        await _context.SaveChangesAsync();

        var actionId = await _context.Actions
            .Where(a => a.ActionName == "Update")
            .Select(a => a.ActionId)
            .FirstAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionId = actionId,
            Resource = "Patient",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return new UpdatePatientResponseDto
        {
            Success = true,
            Message = "Patient updated successfully"
        };
    }
}