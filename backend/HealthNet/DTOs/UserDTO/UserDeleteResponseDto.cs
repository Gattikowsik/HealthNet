using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.UserDTO;

public class UserDeleteResponseDto
{
    public bool Success { get; set; }
    public string? Email { get; set; }
    public AuditLog? AuditLog { get; set; }
    public string? ErrorMessage { get; set; }
}
