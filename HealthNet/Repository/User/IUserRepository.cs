using System;
using HealthNetDb.Entities;

namespace HealthNet.Repository.User;

public interface IUserRepository
{
    Task<IEnumerable<Users>> GetAllUsersAsync();
}