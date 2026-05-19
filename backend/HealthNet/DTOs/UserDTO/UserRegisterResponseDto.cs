using HealthNetDb.Entities;

namespace HealthNet.DTOs;

public class UserRegisterResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public static class UserMappings
{
    public static UserRegisterResponseDto ToUserRegisterResponse(this Users user)
    {
       
        return new UserRegisterResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                RoleName = user.RoleNavigation.RoleName,
                Status = user.Status ? "Active" : "Inactive"
            };

    }
}