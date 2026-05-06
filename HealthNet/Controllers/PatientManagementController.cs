using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthNet.Services.PatientServices;
using HealthNet.DTOs.PatientDto;
using Microsoft.AspNetCore.Authorization;
using HealthNetDb.Entities;
using System.Security.Claims;
using HealthNet.Utility;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientManagementController : ControllerBase
    {
        private readonly IPatientManagementService _patientService;

        public PatientManagementController(IPatientManagementService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPatients([FromQuery] PatientSearchDto searchDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim!);

            var result = await _patientService.SearchPatientsAsync(searchDto, userId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}, {Roles.PublicHealthOfficer}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim!);

            var response = await _patientService.RegisterPatientAsync(dto, userId);
            if (response.Success == false)
            {
                return BadRequest(response.Message);
            }
            return Created(string.Empty, new { patientId = response.PatientId });
        }
        [HttpPatch("{patientId}/deactivate")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivatePatient(int patientId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            try
            {
                bool result = await _patientService.DeactivatePatientAsync(patientId, userId);

                if (!result)
                {
                    return BadRequest("Patient is already inactive.");
                }

                return Ok("Patient deactivated successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found.");
            }
        }
    }
}
