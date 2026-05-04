using System;
using HealthNet.DTOs.MedicalRecordDto;

namespace HealthNet.Services.MedicalServices;

public interface IMedicalRecordService
{
    Task<MedicalRecordResponseDto> AddMedicalRecordAsync(int PatientId, int DoctorId, MedicalRecordRequestDto dto);
}
