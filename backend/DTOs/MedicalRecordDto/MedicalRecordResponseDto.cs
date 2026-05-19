using System;

namespace HealthNet.DTOs.MedicalRecordDto;

public class MedicalRecordResponseDto
{
    public int RecordId { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
}
