using System.Net;
using HealthNet.DTOs.UserDTO;
using HealthNet.Services.UserServices;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HealthNetContext _context;
        private readonly IUserService _userService;

        // <summary>
        // Constructor for initializing fields
        // </summary>
        // <param name="HealthNetContext"> Context class to communicate with DB </param>
        // <param name="configuration"> Configuration object to read the data from appsettings.json </param>
        // <param name="userService"> userService object to use the methods in it. </param>
        public UserController(HealthNetContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }

        // <summary>
        // LoginAsync for User login
        // </summary>
        // <param name="request"> User Login Request DTO for secured data Transfer from client </param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var loginResult = await _userService.LoginServiceAsync(request, _context, _configuration);

            if (!loginResult.Success)
            {
                return Unauthorized(new { error = loginResult.ErrorMessage });
            }

            return Ok(new { token = loginResult.Token });
        }
        /// <summary>
        /// Retrieves all users in the system regardless of their status.
        /// Accessible only by Admin role.
        /// Returns both active (Status = true) and inactive (Status = false) users
        /// along with their respective role information.
        /// Route: GET /api/v1/user/GetAll
        /// </summary>
        /// <returns>
        /// 200 OK — List of all users with count and success flag.
        /// 401 Unauthorized — User is not logged in.
        /// 403 Forbidden — Logged in but does not have Admin role.
        /// 500 Internal Server Error — Unexpected server error.
        /// </returns>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            // Call the service to get all users
            var users = await _userService.GetAllUsersAsync();
            return Ok(new
            {
                // Return a structured response with success status, count of users, and the user data
                success = true,
                count = users.Count(),
                data = users
            });
        }
    }
}
