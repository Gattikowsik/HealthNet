using System;
using HealthNetDb.Data;

namespace HealthNet.Repository.UserRepository;

public class User
{
    private readonly HealthNetContext _context;

    public User(HealthNetContext context)
    {
        _context = context;
    }
}
