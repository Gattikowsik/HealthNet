using System;
using HealthNet.DTOs.MedicalRecordDto;
using System.Security.Claims;
namespace HealthNet.Services.MedicalServices;

public interface IMedicalRecordService
{
    Task<MedicalRecordResponseDto> AddMedicalRecordAsync(int PatientId, int DoctorId, MedicalRecordRequestDto dto);
    Task<Dictionary<DateOnly, List<MedicalRecordGetDto>>> GetPatientRecordsAsync(int patientId,int userId);
}
