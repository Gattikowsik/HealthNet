using HealthNet.DTOs.UserDTO;
using HealthNet.Services.UserServices;
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

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var loginResult = await _userService.LoginServiceAsync(request, _context, _configuration);

            if (!loginResult.Success)
            {
                return Unauthorized(new { error = loginResult.ErrorMessage });
            }

            return Ok(new { token = loginResult.Token });
        }
    }
}
