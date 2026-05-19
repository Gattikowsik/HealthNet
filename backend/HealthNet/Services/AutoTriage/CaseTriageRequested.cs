namespace HealthNet.Services.AutoTriage
{
    public class CaseTriageRequested
    {
        public int ReportId { get; set; }
        public int? CitizenId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
    }
}