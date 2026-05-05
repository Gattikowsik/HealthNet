using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthNetDb.Enums;

namespace HealthNetDb.Entities;

[Table("MedicalRecord")]
public class MedicalRecord
{
    [Key]
    public int RecordId { get; set; }

    [ForeignKey("PatientNavigation")]
    public int PatientId { get; set; }      //Foreign Key from Patient table

    [ForeignKey(nameof(Doctor))]
    public int DoctorId { get; set; }       //Foreign Key from Users table
    public Users? Doctor { get; set; }      //Navigation property

    [Required]
    [Column(TypeName = "VARCHAR(200)")]
    public string Diagnosis { get; set; } = null!;      //Cure or prescription

    [Required]
    [Column(TypeName = "VARCHAR(500)")]
    public string TreatmentPlan { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public MedicalRecordStatus Status { get; set; }     //Active or Inactive

    public virtual Patient PatientNavigation { get; set; } = null!;
}