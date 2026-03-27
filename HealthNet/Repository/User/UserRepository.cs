using System;
using HealthNetDb.Data;

namespace HealthNet.Repository.User;

public class UserRepository
{
    private readonly HealthNetContext _context;

    public UserRepository(HealthNetContext context)
    {
        _context = context;
    }
}
