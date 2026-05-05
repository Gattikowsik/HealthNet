using System.Net;
using System.Security.Claims;
using HealthNet.DTOs.OutbreakMonitoringDTO;
using HealthNet.Repository.OutbreakMonitoringRepository;
using HealthNet.Services.OutbreakMonitoringServices;
using HealthNet.Utility;
using HealthNetDb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}, {Roles.PublicHealthOfficer}")]
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
            return Ok(response);
        }

        //getOutbreakbyId
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOutbreakByIdAsync(int id)
        {
            var response = await _outbreakMonitoringServices.GetOutbreakByIdService(id);

            if (response == null)
                return NotFound("Outbreak not found");

            return Ok(response);
        }

        //Updating the outbreak
        [HttpPatch("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateOutbreakAsync(int id, [FromBody] UpdateOutbreakRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim!);
            var response = await _outbreakMonitoringServices.UpdateOutbreakService(userId, id, request);
            if (!response.Success)
            {
                if (response.Message.Contains("closed"))
                    return Conflict(response.Message);
                if (response.Message.Contains("No changes"))
                    return Conflict(response.Message);
                if (response.Message.Contains("not found"))
                    return NotFound(response.Message);
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        // Add Epidemiology Metrics
        [HttpPut("{outbreakId}/epidemiology")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddEpidemiologyAsync(
            int outbreakId,
            [FromBody] AddEpidemiologyRequestDto request)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var response = await _outbreakMonitoringServices
                .AddEpidemiologyService(userId, outbreakId, request);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response.Message);

                return BadRequest(response.Message);
            }

            return Ok(response);
        }
        // Get ALL Active Outbreaks
        [HttpGet("active")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllActiveOutbreaks()
        {
            var response = await _outbreakMonitoringServices.GetAllActiveOutbreaksService();
            return Ok(response);
        }
    }
}
