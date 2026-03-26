using System;
using HealthNetDb.Entities;

namespace HealthNet.Services.UserServices;

public interface IUserService
{
    string GenerateJwtTokenService(Users user, IConfiguration config);
}
