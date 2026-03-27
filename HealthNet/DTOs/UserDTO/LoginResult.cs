using System;

namespace HealthNet.DTOs.UserDTO;

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
}
