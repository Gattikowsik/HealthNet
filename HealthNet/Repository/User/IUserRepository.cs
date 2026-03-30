using System;
using HealthNet.DTOs;
namespace HealthNet.Repository.User;

public interface IUserRepository
{
    Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto request);
}
