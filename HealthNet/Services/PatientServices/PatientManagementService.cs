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
}