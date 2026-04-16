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
    // Validates if the provided type is one of the allowed lab test types
    public static bool IsValidType(string type)
    {
        // Reject multiple types
        if (type.Contains(",") || type.Contains(" "))
            return false;

        // Explicit check — case sensitive, exact match only
        return type == "Blood" || type == "Swab" || type == "X-Ray";
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

    // Normalizes type to consistent format
    public static string NormalizeType(string type)
    {
        return type.Replace("-", "") switch
        {
            "Blood" => "Blood",
            "Swab" => "Swab",
            "XRay" => "X-Ray",
            _ => type
        };
    }
}
