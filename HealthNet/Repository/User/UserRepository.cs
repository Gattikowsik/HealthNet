using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.User;

public class UserRepository: IUserRepository
{
    private readonly HealthNetContext _context;
    /// <summary>
    /// Constructor for UserRepository, injects HealthNetContext for database access.
    /// </summary>
    /// <param name="context">The database context used for accessing user data.</param>

    public UserRepository(HealthNetContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        try
        {
            // Fetch all users from the database, including their roles
            return await _context.Userss
                .Include(u => u.RoleNavigation)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // If DB query fails, throw with a clear message
            throw new HealthNetException($"An error occurred while fetching users from the database. {ex.Message}");
        }
    }
}