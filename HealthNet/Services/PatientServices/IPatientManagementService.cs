using System;
using HealthNet.DTOs.PatientDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;


namespace HealthNet.Services.PatientServices;

public interface IPatientManagementService
{
     Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto);
     Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto dto);
}