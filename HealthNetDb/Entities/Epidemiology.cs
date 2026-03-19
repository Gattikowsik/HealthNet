using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthNetDb.Entities;

[Table("Epidemiology")]

public class Epidemiology
{
    [Key]
    public int EpiId { get; set; }

    [ForeignKey(nameof(OutbreakNavigation))]
    public int OutbreakId { get; set; }
   
    [Column(TypeName = "VARCHAR(500)")]
    [Required]
    public string MetricsJSON { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool Status { get; set; }  //Active or Monitoring

    public virtual Outbreak OutbreakNavigation{get;set;} = null!;
}