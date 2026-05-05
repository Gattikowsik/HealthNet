using System;

namespace HealthNet.Utility;

public class AuditHelper
{
    // If the request body is null
    public const string BadRequest = "Invalid Request.";

    // If scope is empty
    public const string ScopeRequired = "Scope is required.";

    // If findings is empty
    public const string FindingsRequired = "Findings are required.";

    // If the officer does not exist
    public const string OfficerNotFound = "No officer found with the given ID.";
    // If something unexpected goes wrong
    public const string GenericError = "Something went wrong. Please try again.";
    // If the same officer already audited the same scope
    public const string DuplicateAudit = "An audit already exists for this scope by the same officer.";
    public const string InvalidScope = "Scope must be a meaningful text, not a placeholder or number.";
    public const string InvalidFindings = "Findings must be a meaningful text, not a placeholder or number.";
    public const string StatusRequired = "Status is required. Please provide true or false.";
    // If the audit is not found
    public const string AuditNotFound = "No audit found with the given ID.";

    // If the audit is already closed
    public const string AuditAlreadyClosed = "This audit is already closed.";
}
