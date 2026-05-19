using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("LabTest")]
public class LabTest
{
    [Key]
    public int TestId { get; set; } // Primary key for the LabTest table

    [ForeignKey(nameof(PatientNavigation))]
    public int PatientId { get; set; }      // Foreign key from the Patient Table

    [Column(TypeName = "VARCHAR(20)")]
    [Required]
    public string Type { get; set; } = null!; //Blood/ Swab/ X-Ray

    [Required]
    public DateTime Date { get; set; } 

    public int TechnicianId { get; set; }       // Foreign key from the Users table
    public Users Technician { get; set; } = null!; // Navigation property for the technician who conducted the test

    [Required]
    public bool Status { get; set; } //Pending, Completed 

    // One to One Relationship
    public virtual LabReport LabReportNavigation { get; set; } = null!; 
    public virtual Patient PatientNavigation { get; set; } = null!;
}