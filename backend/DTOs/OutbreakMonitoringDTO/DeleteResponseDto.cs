using System;
namespace HealthNet.DTOs.OutbreakMonitoringDTO;
public class DeleteResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "Record Deleted Successfully";

}
