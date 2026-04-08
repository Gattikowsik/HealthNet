using System;
using HealthNet.DTOs;
using HealthNet.DTOs.UserDTO;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.User;

public class UserRepository : IUserRepository
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
    /// <summary>
    /// Handles the database logic for registering a user, including email uniqueness checks and persistence.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>A response DTO containing the mapped details of the newly created user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request object is null.</exception>
    public async Task<UserRegisterResponseDto> RegisterUser(
            UserRegisterRequestDto request)
    {
        // Email check 
        if (await _context.Userss
            .AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // FETCH ROLE FROM DATABASE
        var role = await _context.Roles
            .SingleOrDefaultAsync(r => r.RoleName == request.RoleName);

        if (role == null)
        {
            throw new ArgumentException(
                $"Invalid role name: {request.RoleName}");
        }

        var user = request.ToUserEntity();
        user.RoleId = role.RoleId;
        user.Status = true;

        _context.Userss.Add(user);
        await _context.SaveChangesAsync();

        await _context.Entry(user).Reference(u => u.RoleNavigation).LoadAsync();
        return user.ToUserRegisterResponse();
    }
    /// <summary>
    /// Retrieves a user entity from the database based on the provided email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user entity corresponding to the provided email, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the email parameter is null or empty.</exception>
    public async Task<Users> GetUserByEmailAsync(string email)
    {
        return await _context.Userss.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Retrieves all users from the database, including both active and inactive users, along with their associated roles.
    /// </summary>
    /// <returns>A collection of all users with their associated roles.</returns>
    /// <exception cref="HealthNetException"></exception>
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

    //Get user by id
    /// <summary>
    /// Retrieves a user entity from the database based on the provided user ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// The user entity corresponding to the provided ID, or null if not found.
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<Users?> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Userss
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(u => u.UserId == id);
            return user;
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }

    /// <summary>
    /// Updates the details of an existing user in the database.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>
    ///  It updates the user entity in the database based on the provided user.
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task UpdateUserAsync(Users user)
    {
        try
        {
            _context.Userss.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }

    /// <summary>
    /// Getting Action Id using Action Name from the database.
    /// </summary>
    /// <param name="acttionName"></param>
    /// <returns>
    ///  It returns the Id of the Action from the Database.
    /// </returns>
    /// <exception cref="HealthNetException"></exception>    
    public async Task<int> GetActionIdAsync(String actionName)
    {
        try
        {
            HealthNetDb.Entities.Action action = await (from ac in _context.Actions where ac.ActionName == actionName select ac).FirstAsync();
            return action.ActionId;
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }

    /// <summary>
    /// Inserts the Data into AuditLog table in the Database.
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="userId"></param>
    /// <param name="role"></param>
    /// <returns>
    ///  It returns the AuditLog object which is inserted into the table.
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<AuditLog> InsertIntoAuditLogAsync(int actionId, int userId, string role)
    {
        try
        {
            AuditLog auditLog = new AuditLog();
            auditLog.ActionId = actionId;
            auditLog.UserId = userId;
            auditLog.Timestamp = DateTime.Now;
            auditLog.Resource = role;
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }

    /// <summary>
    /// DeActivates the User in the Database
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>
    ///  The User who got DeActivated.
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<Users> UpdateUserStatusAsync(int userId)
    {
        try
        {
            Users? user = await GetUserByIdAsync(userId);
            if (user.Status == false)
            {
                throw new HealthNetException("No Active user found with this User Id.");
            }
            if (user != null)
                user.Status = false;
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }
}
