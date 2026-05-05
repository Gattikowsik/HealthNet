using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("LabReport")]
public class LabReport
{
    [Key]
    public int ReportId { get; set; } // Primary key for the LabReport table

    [ForeignKey(nameof(LabTestNavigation))]
    public int TestId { get; set; }        // Foreign key from LabTest

    [Column(TypeName = "VARCHAR(500)")]
    [Required]
    public string FileURI { get; set; } = null!;    //Report file URL

    [Column(TypeName = "VARBINARY(MAX)")]
    [Required]
    public byte[] FileData { get; set; } = null!;   // Actual file data stored in the database

    [Required]
    public DateTime Date { get; set; } //Date & Time of the test

    [Required]
    public bool Status { get; set; } //Not verified, verified

    // One to One Relationship building
    public virtual LabTest LabTestNavigation { get; set; } = null!;
}