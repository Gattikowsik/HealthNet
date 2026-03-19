using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("SymptomReport")]
public class SymptomReport
{
    [Key]
    public int ReportId { get; set; }

    [ForeignKey(nameof(Citizen))]
    public int CitizenId { get; set; }      //Foreign Key from Users table
    public Users Citizen { get; set; } = null!;   //Navigation Property

    [Column(TypeName = "VARCHAR(300)")]
    [Required]
    public string SymptomsJson { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool Status { get; set; }
}