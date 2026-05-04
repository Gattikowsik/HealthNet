namespace HealthNet.DTOs.OutbreakMonitoringDTO
{
    public class AddEpidemiologyResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? EpiId { get; set; }
    }
}