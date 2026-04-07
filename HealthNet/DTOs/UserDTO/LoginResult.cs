using System;
using HealthNetDb.Entities;

namespace HealthNet.DTOs.UserDTO;

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public AuditLog? Auditlog { get; set; }
    public string? ErrorMessage { get; set; }
}
