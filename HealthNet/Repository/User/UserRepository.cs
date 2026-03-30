using System;
using HealthNet.DTOs;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;
namespace HealthNet.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly HealthNetContext _context;

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

            await _context.Entry(user)
                .Reference(u => u.RoleNavigation)
                .LoadAsync();
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

    public async Task UpdateUserAsync(Users user)
    {
         _context.Userss.Update(user);
            await _context.SaveChangesAsync();
    }
}

