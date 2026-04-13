using System;
using HealthNet.DTOs.PateintDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;
namespace HealthNet.Repository.PatientRepository;

public interface IPatientManagementRepository
{
    Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto);

}
