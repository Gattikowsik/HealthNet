using System;

namespace HealthNet.Utility;

public class AuditHelper
{
    public const string BadRequest       = "Invalid Request.";
    public const string ScopeRequired    = "Scope is required.";
    public const string InvalidScope     = "Scope must be meaningful text, not a placeholder or number.";
    public const string FindingsRequired = "Findings are required.";
    public const string InvalidFindings  = "Findings must be meaningful text, not a placeholder or number.";
    public const string StatusRequired   = "Status is required. Please provide true or false.";
    public const string OfficerNotFound  = "No officer found with the given ID.";
    public const string DuplicateAudit   = "An audit already exists for this scope by the same officer.";
    public const string AuditNotFound    = "No audit found with the given ID.";
    public const string AuditAlreadyClosed = "This audit is already closed and cannot be updated.";
    public const string NoAuditsFound    = "No audits found for the given filters.";
    public const string GenericError     = "Something went wrong. Please try again.";
}
