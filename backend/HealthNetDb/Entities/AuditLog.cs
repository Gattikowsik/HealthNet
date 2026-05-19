using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("AuditLog")]
public class AuditLog
{
    [Key]
    public int AuditId { get; set; }

    [ForeignKey("UsersNavigation")]     //Maps to Table Users
    public int UserId { get; set; }     // Foriegn key from the Users table

    [Required]
    [ForeignKey("ActionNavigation")]     //Maps to Table Action
    public int ActionId { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(100)")]
    public string Resource { get; set; } = null!;  //non nullable

    [Required]
    public DateTime Timestamp { get; set; }  //Non nullable

    public virtual Users? UsersNavigation { get; set; }   //Navigation property to Users
    
    public virtual Action? ActionNavigation { get; set; }  //Navigation property to Action
}



