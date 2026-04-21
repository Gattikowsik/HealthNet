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
    // Valid status values
    public static readonly string[] ValidStatuses = { "Pending", "Completed" };
    // Error messages
    public static string InvalidTypeMessage => $"Invalid test type. Must be one of: {string.Join(", ", GetValidTypes())}.";
    public static string InvalidStatusMessage => $"Invalid status. Must be one of: {string.Join(", ", ValidStatuses)}.";
    public static string FutureDateMessage => "Date filter cannot be a future date.";

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

    // Checks if the given status is valid
    public static bool IsValidStatus(string status)
    {
        return status == "Pending" || status == "Completed";
    }

    // Checks if the given date is in the future
    public static bool IsFutureDate(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.UtcNow);
    }
    
    // Returns current UTC datetime
    public static DateTime GetUTCDateTime()
    {
        return DateTime.UtcNow;
    }
}
