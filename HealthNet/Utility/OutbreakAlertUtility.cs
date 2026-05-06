using System.Text.Json;

namespace HealthNet.Utility
{
    public static class OutbreakAlertUtility
    {
        private const double RtThreshold = 1.3;
        public static bool IsThresholdBreached(double rtNow)
        {
                        return rtNow > RtThreshold;
        }
    }
}