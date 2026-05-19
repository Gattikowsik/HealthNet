using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("Role")]
public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string RoleName { get; set; } = null!;
    // Different Roles : [Citizen/Doctor/Lab/PublicHealth/Researcher/Admin/Compliance]
}
