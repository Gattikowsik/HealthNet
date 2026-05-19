using System;

namespace HealthNet.Utility;

public class PasswordHelper
{
    public static (bool IsValid, string Message) ValidatePassword(string password)
    {
        // checking whether the password is not null or whitespace
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty.");
        // minimum character length requirement
        if (password.Length < 8)
            return (false, "Minimum 8 characters required.");
        // at least one uppercase letter (A-Z)
        if (!password.Any(char.IsUpper))
            return (false, "At least one uppercase letter required.");
        // at least one lowercase letter (a-z)
        if (!password.Any(char.IsLower))
            return (false, "At least one lowercase letter required.");
         // at least one numeric digit (0-9)
        if (!password.Any(char.IsDigit))
            return (false, "At least one numeric digit required.");
        // at least one special character 
        if (!password.Any(char.IsPunctuation))
            return (false, "At least one Special character is required.");
        // Return success if all security checks pass
            return (true, "Success");
    }
}
