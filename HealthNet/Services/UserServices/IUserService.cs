using System;
using HealthNet.DTOs;
using HealthNet.DTOs.UserDTO;
using HealthNetDb.Data;
using HealthNetDb.Entities;

namespace HealthNet.Services.UserServices;

public interface IUserService
{
  Task<LoginResult> LoginServiceAsync(UserLoginRequest request, HealthNetContext _context, IConfiguration _config);
  Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto request);
  Task<(bool success, string message)> ResetPasswordAsync(ForgotPasswordDto dto);
  Task<IEnumerable<UserResponse>> GetAllUsersAsync();
  Task<UserResponse> GetUserByIdAsync(int id);
  Task<UserResponse> UpdateUserAsync(int id, UpdateUserDto dto);
}
