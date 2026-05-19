using System;

namespace HealthNet.Utility;

public class CasesHelper
{
    // If the request body is null
    public const string BadRequest = "Invalid Request.";

    // If the ReportId does not exist in SymptomReport table
    public const string ReportNotFound = "No symptom report found with the given ReportId.";

    // If diagnosis is empty
    public const string DiagnosisRequired = "Diagnosis is required.";

    // If diagnosis is a placeholder or pure number
    public const string InvalidDiagnosis = "Diagnosis must be meaningful text, not a placeholder or number.";

    // If status is not provided
    public const string StatusRequired = "Status is required. Please provide true or false.";

    // If case already exists for the same citizen
    public const string DuplicateCase = "A case already exists for this symptom report.";

    // If case is not found
    public const string CaseNotFound = "No case found with the given ID.";

    // If case is already recovered — cannot update diagnosis
    public const string CaseAlreadyRecovered = "This case is already recovered and cannot be updated.";
    
    // If something unexpected goes wrong
    public const string GenericError = "Something went wrong. Please try again.";
}
