using HealthNetDb.Entities;
using System.Text.Json;

namespace HealthNet.Services.AutoTriage
{
    public class SymptomRiskEvaluator : ISymptomRiskEvaluator
    {
        public bool IsHighRisk(SymptomReport report, out string reason)
        {
            reason = string.Empty;

            if (string.IsNullOrWhiteSpace(report.SymptomsJson))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(report.SymptomsJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("vitals", out var vitals))
                    return false;

                if (vitals.TryGetProperty("temperature", out var temp))
                {
                    var value = temp.GetDouble();
                    Console.WriteLine($"[AUTO‑TRIAGE] Temperature = {value}");

                    if (value >= 102)
                    {
                        reason = "High fever detected";
                        return true;
                    }
                }

                if (vitals.TryGetProperty("oxygenLevel", out var ox))
                {
                    var oxygen = ox.GetInt32();
                    Console.WriteLine($"[AUTO‑TRIAGE] Oxygen = {oxygen}");

                    if (oxygen < 94)
                    {
                        reason = "Low oxygen level detected";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUTO‑TRIAGE ERROR] {ex.Message}");
                return false;
            }

            return false;
        }
    }
}