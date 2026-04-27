using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace HealthNet.Utility;

public static class LabReportHelper
{
    // Valid file extensions
    private static readonly string[] ValidExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

    // Validates if the uploaded file has a valid extension
    public static bool IsValidFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        return ValidExtensions.Contains(extension);
    }

    // Generates SHA256 hash of actual file content
    public static async Task<string> GenerateFileHashAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var bytes = await SHA256.Create().ComputeHashAsync(stream);
        return Convert.ToHexString(bytes).ToLower();    // 64 char hex string
    }

    // Error messages
    public static string InvalidFileMessage => $"File must be one of: {string.Join(", ", ValidExtensions)}.";
    public static string FileTooLargeMessage => "File size must not exceed 10MB.";
    public static string TestNotFoundMessage => "Lab test not found.";
    public static string TestNotPendingMessage => "Lab test is already completed. Cannot upload report.";
    public static string DuplicateReportMessage => "A report already exists for this lab test.";
    public static string UnauthorizedTechnicianMessage => "You are not the assigned technician for this lab test.";
    public static string InvalidTestIdMessage => "TestId must be greater than 0.";
    public static string NoReportsFoundMessage(int testId) => $"No reports found for Test ID {testId}.";
}
