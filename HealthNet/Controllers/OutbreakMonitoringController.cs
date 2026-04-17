using System.Net;
using System.Security.Claims;
using HealthNet.DTOs.OutbreakMonitoringDTO;
using HealthNet.Repository.OutbreakMonitoringRepository;
using HealthNet.Services.OutbreakMonitoringServices;
using HealthNetDb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/outbreaks")]
    [ApiController]
    [Authorize(Roles = "Admin,Doctor,Public Health Officer")]
    public class OutbreakMonitoringController : ControllerBase
    {
        private readonly IOutbreakMonitoringServices _outbreakMonitoringServices;
        public OutbreakMonitoringController(IOutbreakMonitoringServices outbreakMonitoringServices)
        {
            _outbreakMonitoringServices = outbreakMonitoringServices;
        }

        // <summary>
        // CreateOutbreakAsync — Create Outbreaks for submitted details by autheticated User(Doctor,Admin,Public Health Officer)
        // </summary>
        // <param name="request"> CreateOutbreakRequestDto DTO for data transfer from client </param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateOutbreakAsync([FromBody] CreateOutbreakRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim!);
            var response = await _outbreakMonitoringServices.AddOutbreakService(userId, request);
            if (!response.Success || response == null)
            {
                return BadRequest(response?.Message);
            }
            return Created($"/api/outbreaks/{response.OutbreakId}", new
            {
                message = response.Message,
                OutbreakId = response.OutbreakId
            }
            );
        }
    }
}
