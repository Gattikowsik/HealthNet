using System;

namespace HealthNet.Utility;

public class ForgotPasswordHelper
{
    //If passwords do not match, return this message
    public const string PasswordsDoNotMatch ="Passwords do not match";
    // If the password does not meet the security requirements, return this message
    public const string InvalidPassword ="Password must be at least 8 characters, include one uppercase letter and one number";
    // If the user is not found in the database, return this message
    public const string UserNotFound ="User not found";
    // If the password is successfully updated, return this message
    public const string PasswordUpdatedSuccess = "Password updated successfully";
    // If there is a generic error during the password reset process, return this message
    public const string GenericError = "Something went wrong. Please try again.";
    // If the request is invalid, return this message
    public const string BadRequest = "Invalid Request";
}

