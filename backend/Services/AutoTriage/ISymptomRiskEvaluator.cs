using HealthNetDb.Entities;

namespace HealthNet.Services.AutoTriage
{
    public interface ISymptomRiskEvaluator
    {
        bool IsHighRisk(SymptomReport report, out string reason);
    }
}