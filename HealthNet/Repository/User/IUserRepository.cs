using System;
using System.Runtime.CompilerServices;
using HealthNet.DTOs;
using HealthNetDb.Entities;
namespace HealthNet.Repository.User;

public interface IUserRepository
{
    Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto request);
    Task<IEnumerable<Users>> GetAllUsersAsync();
    Task<Users> GetUserByEmailAsync(string email);
    Task<Users?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(Users user);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<int> GetActionIdAsync(String actionName);
    Task<AuditLog> InsertIntoAuditLogAsync(int actionId, int userId, string role);
}
