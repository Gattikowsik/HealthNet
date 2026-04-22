using System;
using HealthNet.DTOs.PatientDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;
using HealthNetDb.Data;
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

        bool patientExists = await _context.Patients.AnyAsync(p =>
            p.Name == dto.Name &&
            p.ContactInfo == dto.ContactInfo
        );

        if (patientExists)
        {
            return new RegisterPatientResponseDto
            {
                Success = false,
                Message = "A patient with the same name and contact number already exists."
            };
        }
        var patient = new Patient
        {
            Name = dto.Name,
            DOB = dto.DOB,
            Gender = dto.Gender,
            Address = dto.Address,
            ContactInfo = dto.ContactInfo,
            Status = dto.Status
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