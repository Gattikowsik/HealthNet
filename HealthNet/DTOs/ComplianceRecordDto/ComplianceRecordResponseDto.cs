using System;

namespace HealthNet.DTOs.ComplianceRecordDto;

public class ComplianceRecordResponseDto
{
    // This is what we send BACK after saving
    // We only return the ComplianceId as per the acceptance criteria
    public int ComplianceId { get; set; }
}
