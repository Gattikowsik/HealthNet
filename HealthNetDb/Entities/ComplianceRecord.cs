using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("ComplianceRecord")]
public class ComplianceRecord
{
    [Key]
    public int ComplianceId { get; set; }

    [Required]
    public int EntityId { get; set; } //this is id's of either(caseid, testid, outbreakid)

    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string Type { get; set; } = null!; //[case, test, oubreak]

    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string Result { get; set; } = null!; //[resolved/ pending/....]

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(200)")]
    public string Notes { get; set; } = null!;
}
