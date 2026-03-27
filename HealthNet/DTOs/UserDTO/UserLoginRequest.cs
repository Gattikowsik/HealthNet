using System;

namespace HealthNet.DTOs.UserDTO;

public class UserLoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
