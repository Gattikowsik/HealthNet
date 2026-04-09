using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthNetDb.Enums;

namespace HealthNetDb.Entities;

[Table("Patient")]
public class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string Name { get; set; } = null!;

    [Required]
    public DateOnly DOB { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(10)")]
    public string Gender { get; set; } = null!;

    [Required]
    [Column(TypeName = "VARCHAR(100)")]
    public string Address { get; set; } = null!;

    [Required]
    [Column(TypeName = "VARCHAR(15)")]
    public string ContactInfo { get; set; } = null!;

    [Required]
    public PatientStatus Status { get; set; } 
}
