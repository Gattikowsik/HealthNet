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

    //Returns current IST datetime.
    public static DateTime GetISTDateTime()
    {
        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);
    }
}
