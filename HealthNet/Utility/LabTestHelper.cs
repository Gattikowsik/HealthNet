using System;

namespace HealthNet.Utility;

public static class LabTestHelper
{
    public static readonly string[] ValidTypes = { "Blood", "Swab", "X-Ray" };

    // Checks if the given test type is valid.
    public static bool IsValidType(string type)
    {
        return ValidTypes.Contains(type);
    }

    // Checks if the given date is in the future.
    public static bool IsFutureDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
