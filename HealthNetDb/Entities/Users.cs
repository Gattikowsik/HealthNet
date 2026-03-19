using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthNetDb.Entities;

[Table("Users")]
public class Users
{
    [Key]
    public int UserId { get; set; }     //A person's userId

    [Column(TypeName = "VARCHAR(50)")]
    [Required]
    public string Name { get; set; } = null!;  //User Name

    [ForeignKey("RoleNavigation")]      //Maps to Table Role 
    public int RoleId { get; set; }

    [Column(TypeName = "VARCHAR(50)")]
    [Required]
    public string Email { get; set; } = null!;   //User's EmailId

    [Column(TypeName = "VARCHAR(15)")]
    [Required]
    public string Phone { get; set; } = null!;      //User's Phone Number

    [Required]
    public bool Status { get; set; }    //User might be active or inactive

    [Required]
    [Column(TypeName = "VARCHAR(50)")]
    public string Password { get; set; } = null!;  //Password Hash for security

    public virtual Role? RoleNavigation { get; set; }  //Navigation property to Role
}

