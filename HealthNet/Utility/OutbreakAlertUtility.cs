using System.Text.Json;

namespace HealthNet.Utility
{
    public static class OutbreakAlertUtility
    {
        private const double RtThreshold = 1.3;

        public static bool IsThresholdBreached(
            string metricsJson,
            out double rt)
        {
            rt = 0;

            if (string.IsNullOrWhiteSpace(metricsJson))
                return false;

            using var doc = JsonDocument.Parse(metricsJson);

            if (!doc.RootElement.TryGetProperty("Rt", out var rtProp))
                return false;

            rt = rtProp.GetDouble();

            return rt > RtThreshold;
        }
    }
}