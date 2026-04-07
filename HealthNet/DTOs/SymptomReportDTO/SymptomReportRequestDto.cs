using System;
using System.ComponentModel.DataAnnotations;
namespace HealthNet.DTOs;

public class SymptomReportRequestDto
{
    /// <summary>
    /// Structured symptom data sent as a JSON string.
    /// Example:
    /// {
    ///   "commonSymptoms": { "fever": true, "cough": false },
    ///   "vitals": { "temperature": 101 },
    ///   "otherSymptoms": "Body pain at night"
    /// }
    /// </summary>
    [Required]
    public string SymptomsJson { get; set; } = null!;

    /// <summary>
    /// Date when the symptoms were observed.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

}
