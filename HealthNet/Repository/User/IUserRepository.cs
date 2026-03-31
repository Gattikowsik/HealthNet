using System;
using HealthNet.DTOs;
using HealthNetDb.Entities;
namespace HealthNet.Repository.User;

public interface IUserRepository
{
    Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto request);

    Task<Users> GetUserByEmailAsync(string email);
    Task UpdateUserAsync(Users user);
}
