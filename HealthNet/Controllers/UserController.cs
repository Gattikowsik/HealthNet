using HealthNet.Services.UserServices;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/auth/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HealthNetContext _context;
        private readonly IUserService _userService;


        public UserController(HealthNetContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }


        //Jwt Token Generation
        private string GenerateJwtToken(Users user)
        {
            string token = _userService.GenerateJwtTokenService(user,_configuration);
            return token;  
        }
    }
}
