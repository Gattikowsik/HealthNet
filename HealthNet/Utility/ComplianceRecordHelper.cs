
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
    // If a compliance record already exists for the same EntityId and Type
    public const string DuplicateRecord = "A compliance record already exists for this entity.";
    // If no compliance records are found for the given filters
    public const string NoRecordsFound = "No compliance records found for the given filters.";
    // If no filters are provided at all
    public const string NoFiltersProvided = "Please provide at least one filter to search.";
}


