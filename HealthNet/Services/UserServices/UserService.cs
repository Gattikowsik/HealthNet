using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using HealthNet.DTOs.UserDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HealthNet.Services.UserServices;

public class UserService : IUserService
{
    // Login Service
    public async Task<LoginResult> LoginServiceAsync(UserLoginRequest request, HealthNetContext _context, IConfiguration _config)
    {
        // Check whether email and password are non empty fields
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Email and Password are required."
            };
        }
        // Validate the Email and user status
        Users? user = await _context.Userss.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !user.Status)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "No active account found with the provided email address."
            };
        }
        // Validate the password
        bool isPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isPassword)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Invalid Password."
            };
        }
        // Generate token
        var token = await GenerateJwtTokenServiceAsync(user, _config);
        return new LoginResult
        {
            Success = true,
            Token = token,
        };
    }

    //Jwt Token Generation
    // <summary>
    // GenerateJwtToken for generating the token
    // </summary>
    // <param name="user">user object for DB communication </param>
    private async Task<string> GenerateJwtTokenServiceAsync(Users user, IConfiguration _config)
    {

        // Jwt token variables
        var secretKey = _config["JwtSettings:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }
        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        var expiryMinutes = int.TryParse(_config["JwtSettings:ExpiryMinutes"], out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));  //Converting plain secret key into Cryptographic key object
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  //Create a Unique signing key using secret key

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleNavigation?.RoleName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));        //Token Generated with Paylaod
    }
}
