using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthNetDb.Entities;

[Table("Cases")]
public class Cases
{
    [Key]
    public int CaseId { get; set; }

    [ForeignKey(nameof(Citizen))]
    public int CitizenId { get; set; }      //Foreign Key from Users table  
    public Users Citizen { get; set; } = null!;     //Navigation Property

    [ForeignKey(nameof(Doctor))]
    public int DoctorId { get; set; }       //Foreign Key from Users table
    public Users Doctor { get; set; } = null!;  //Navigation Property

    [Required]
    [Column(TypeName = "VARCHAR(200)")]
    public string Diagnosis { get; set; } = null!;     //Cure (Prescription)

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool Status { get; set; }    //Recovered or under treatment
}