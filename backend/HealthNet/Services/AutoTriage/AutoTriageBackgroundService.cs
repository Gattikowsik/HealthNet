using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace HealthNet.Services.AutoTriage
{
    public class AutoTriageBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AutoTriageBackgroundService> _logger;

        public AutoTriageBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AutoTriageBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider
                    .GetRequiredService<HealthNetContext>();

                var evaluator = scope.ServiceProvider
                    .GetRequiredService<ISymptomRiskEvaluator>();

                var pendingReports = await context.SymptomReports
                    .Where(r => r.Status == SymptomStatus.Submitted)
                    .ToListAsync(stoppingToken);

                foreach (var report in pendingReports)
                {
                    if (evaluator.IsHighRisk(report, out var reason))
                    {
                        // ✅ Escalate status
                        report.Status = SymptomStatus.UnderReview;

                        // ✅ Publish CaseTriageRequested (log for now)
                        var triageEvent = new CaseTriageRequested
                        {
                            ReportId = report.ReportId,
                            CitizenId = report.CitizenId,
                            Reason = reason
                        };

                        _logger.LogInformation(
                            "CaseTriageRequested: {@Event}",
                            triageEvent);
                    }
                }

                await context.SaveChangesAsync(stoppingToken);

                // Run every 1 minute (reduce for testing)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}