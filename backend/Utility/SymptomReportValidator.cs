using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using HealthNetDb.Entities;

namespace HealthNet.Utility
{
    public static class SymptomReportValidator
    {
        public static void ValidateSymptomsJson(string symptomsJson)
        {
            if (string.IsNullOrWhiteSpace(symptomsJson))
                throw new HealthNetException("SymptomsJson cannot be empty.");

            try
            {
                using var doc = JsonDocument.Parse(symptomsJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("vitals", out var vitals))
                    throw new HealthNetException("Missing 'vitals' section.");

                if (!vitals.TryGetProperty("temperature", out _))
                    throw new HealthNetException("Missing 'temperature' in vitals.");

                if (!vitals.TryGetProperty("oxygenLevel", out _))
                    throw new HealthNetException("Missing 'oxygenLevel' in vitals.");
            }
            catch (JsonException)
            {
                throw new HealthNetException("SymptomsJson is not valid JSON.");
            }
        }

        public static void ValidateDate(DateTime date)
        {
            if (date.Date > DateTime.UtcNow.Date)
                throw new HealthNetException("Date cannot be in the future.");
        }

        public static void ValidateLocation(string? location)
        {
            if (location != null && location.Length > 200)
                throw new HealthNetException("Location is too long.");
        }
    }
}
