using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace HealthNetDb.Entities;

[Table("Audit")]
public class Audit
{
    [Key]
    public int AuditId { get; set; }
    
    [ForeignKey(nameof(Officer))]
    public int OfficerId { get; set; }      //foreign key from userid and role is checked by the roleid
    public Users? Officer { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(100)")]
    public string Scope { get; set; } = null!;  //which area(like testing,missing records,.) is inspected by PHOfficer

    [Required]
    [Column(TypeName = "VARCHAR(200)")]
    public string Findings { get; set; } = null!;  //what findings are done in that area(like testing,missing records,..)

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool Status { get; set; }  //Compliant or non compliant
}
