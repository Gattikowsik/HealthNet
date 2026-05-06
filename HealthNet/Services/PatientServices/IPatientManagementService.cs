using System;
using HealthNet.DTOs.PatientDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;


namespace HealthNet.Services.PatientServices;

public interface IPatientManagementService
{
     Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto, int userId);
     Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto dto, int userId);
     Task<bool> DeactivatePatientAsync(int patientId, int userId);
}