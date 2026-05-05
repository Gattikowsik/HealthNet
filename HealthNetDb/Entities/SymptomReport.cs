using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

public enum SymptomStatus
{
    Submitted = 1,
    UnderReview = 2,
    Reviewed = 3,
    Closed = 4
}
[Table("SymptomReport")]
public class SymptomReport
{
    [Key]
    public int ReportId { get; set; }

    [ForeignKey(nameof(Citizen))]
    public int? CitizenId { get; set; }      //Foreign Key from Users table
    public Users Citizen { get; set; } = null!;   //Navigation Property

    [MaxLength]
    [Required]
    public string SymptomsJson { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public SymptomStatus Status { get; set; }
    public bool IsDeleted { get; set; }  // Soft delete flag
}