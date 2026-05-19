
using System;

namespace HealthNet.Utility;

public class ComplianceHelper
{
    public const string InvalidType           = "Invalid type. Allowed values: case, test, outbreak.";
    public const string InvalidResult         = "Invalid result. Allowed values: compliant, non compliant, partially compliant, pending review.";
    public const string EntityNotFound        = "No record found with the given ID for the specified type.";
    public const string UserNotFound          = "No user found with the given UserId.";
    public const string BadRequest            = "Invalid Request.";
    public const string GenericError          = "Something went wrong. Please try again.";
    public const string DuplicateRecord       = "A compliance record already exists for this entity.";
    public const string NoRecordsFound        = "No compliance records found for the given filters.";
    public const string NoFiltersProvided     = "Please provide at least one filter to search.";
    public const string RecordNotFound        = "No compliance record found with the given ID.";
    public const string CannotUpdateCompliant = "This compliance record is compliant and cannot be updated.";
    public const string CannotUpdateDeleted   = "This compliance record has been deleted and cannot be updated.";
    public const string AlreadyDeleted        = "This compliance record is already deleted.";
    public const string NotesRequired         = "Notes are required.";
}


