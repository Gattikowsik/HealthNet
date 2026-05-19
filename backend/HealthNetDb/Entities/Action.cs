using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace HealthNetDb.Entities;
 
[Table("Action")]
public class Action
{
    [Key]
    public int ActionId { get; set; }
 
    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string ActionName { get; set; } = null!;  //non nullable
}
 