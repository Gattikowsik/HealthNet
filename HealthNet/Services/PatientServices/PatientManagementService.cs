using System;
using HealthNet.DTOs.PateintDto;
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

    public PatientManagementService(IPatientManagementRepository patientRepository,HealthNetContext context)
    {
        _patientRepository = patientRepository;
        _context=context;
    }

    public async Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto)
    {
        return await _patientRepository.SearchPatientsAsync(searchDto);
    }
}
