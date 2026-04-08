
using System;

namespace HealthNet.Utility;

public class ComplianceHelper
{
    // If the type is not case, test or outbreak
    public const string InvalidType = "Invalid type. Allowed values: case, test, outbreak.";

    // If the result is not one of the 4 allowed values
    public const string InvalidResult = "Invalid result. Allowed values: compliant, non compliant, partially compliant, pending review.";

    // If the EntityId does not exist in the respective table
    public const string EntityNotFound = "No record found with the given ID for the specified type.";
    // If the UserId does not exist in Users table
    public const string UserNotFound = "No user found with the given UserId.";

    // If the request body is null
    public const string BadRequest = "Invalid Request.";

    // If something unexpected goes wrong
    public const string GenericError = "Something went wrong. Please try again.";
}
