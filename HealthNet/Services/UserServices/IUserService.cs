using System;
using HealthNet.DTOs.UserDTO;
using HealthNetDb.Data;
using HealthNetDb.Entities;

namespace HealthNet.Services.UserServices;

public interface IUserService
{
    Task<string> GenerateJwtTokenServiceAsync(Users user, IConfiguration config);
    Task<LoginResult> LoginServiceAsync(UserLoginRequest request, HealthNetContext _context, IConfiguration _config);
}
