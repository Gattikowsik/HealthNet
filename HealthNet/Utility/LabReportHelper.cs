using System;
using System.Security.Cryptography;
using System.Text;

namespace HealthNet.Utility;

public static class LabReportHelper
{
    // Validates if the given URI is a valid URL or relative path
    public static bool IsValidFileUri(string fileUri)
    {
        // Check if it's a valid cloud URL (http/https)
        if (Uri.TryCreate(fileUri, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return true;
        }

        // Check if it's a valid relative path
        // Must start with / and end with valid file extension
        if (fileUri.StartsWith("/") &&
            (fileUri.EndsWith(".pdf") ||
             fileUri.EndsWith(".jpg") ||
             fileUri.EndsWith(".jpeg") ||
             fileUri.EndsWith(".png")))
        {
            return true;
        }

        return false;
    }

    // Generates SHA256 hash of the given file URI
    public static string GenerateFileHash(string fileUri)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(fileUri));
        return Convert.ToHexString(bytes).ToLower();    // 64 char hex string
    }

    // Error messages
    public static string InvalidUrlMessage => "FileURI must be a valid cloud URL (http/https) or local path (e.g. /reports/file.pdf).";
    public static string TestNotFoundMessage => "Lab test not found.";
    public static string TestNotPendingMessage => "Lab test is already completed. Cannot upload report.";
    public static string DuplicateReportMessage => "A report already exists for this lab test.";
    public static string UnauthorizedTechnicianMessage => "You are not the assigned technician for this lab test.";
}
