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

    public async Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto)
    {
        return await _patientRepository.SearchPatientsAsync(searchDto);
    }

    public async Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto dto)
    {
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

        return new RegisterPatientResponseDto
        {
            PatientId = saved.PatientId
        };
    }
}