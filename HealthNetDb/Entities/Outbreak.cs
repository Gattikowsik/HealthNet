using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace HealthNetDb.Entities;

[Table("Outbreak")]

public class Outbreak
{
    [Key]
    public int OutbreakId { get; set; }

    [Column(TypeName = "VARCHAR(50)")]
    [Required]
    public string Disease { get; set; } = null!;

    [Column(TypeName = "VARCHAR(100)")]
    [Required]
    public string Location { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "VARCHAR(50)")]
    [Required]
    public string Severity { get; set; } = null!;

    [Required]
    public bool Status { get; set; }     //active or inactive
}