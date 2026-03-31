using System.Net;
using HealthNet.DTOs;
using HealthNet.DTOs.UserDTO;
using HealthNet.Services.UserServices;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;
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

        // <summary>
        // Register method for new user
        // </summary>
        /// <param name="user">The user registration data transfer object containing credentials and profile info.</param>
        /// <returns>An IActionResult containing the registration response or an error message.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserRegisterResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto request)
        {
            try{
                // Validate Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            var result = await _userService.RegisterUser(request);
            return Ok(result);
        }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // <summary>
        // ForgotPassword method for resetting the password of the user
        // </summary>
        /// <param name="request">The forgot password data transfer object containing the email and new password.</param>
        /// <returns>An IActionResult indicating the success or failure of the password reset operation.</returns>
        [HttpPost("forgotpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            try
            {
                if(request == null)
                {
                    return BadRequest(ForgotPasswordHelper.InvalidPassword);
                }
                // Call the service to do the actual work
                var (success, message) = await _userService.ResetPasswordAsync(request);

                // User doesn't exist
                if (!success)
                {
                    return BadRequest(message); 
                }

                return Ok(new { message });
            }
            catch
            {
                // Handle exception if any 
                return StatusCode(500, ForgotPasswordHelper.GenericError);
            }
        }





    }
}
