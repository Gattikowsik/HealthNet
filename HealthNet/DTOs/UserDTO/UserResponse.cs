using System;

namespace HealthNet.DTOs.UserDTO;

public class UserResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool Status { get; set; }                // true = Active, false = Inactive
    public string RoleName { get; set; } = null!;   // "Admin", "Doctor" etc.
}