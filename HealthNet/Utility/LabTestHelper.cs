using System;

namespace HealthNet.Utility;

public enum LabTestType
{
    Blood,
    Swab,
    XRay    // X-Ray stored as XRay in enum
}
public static class LabTestHelper
{
    // Checks if the given test type is valid using enum
    public static bool IsValidType(string type)
    {
        return Enum.TryParse<LabTestType>(type.Replace("-", ""), ignoreCase: false, out _);
    }

    // Returns valid types from enum
    public static string[] GetValidTypes()
    {
        return Enum.GetNames(typeof(LabTestType))
                   .Select(t => t.Replace("XRay", "X-Ray"))
                   .ToArray();
    }

    // Returns current UTC datetime
    public static DateTime GetUTCDateTime()
    {
        return DateTime.UtcNow;
    }
}
