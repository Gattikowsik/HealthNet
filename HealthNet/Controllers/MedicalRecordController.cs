using HealthNet.Services.MedicalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthNet.DTOs.MedicalRecordDto;
using Microsoft.AspNetCore.Authorization;
using HealthNet.Utility;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
        {
            _service = service;
        }
        // ✅ POST /api/patients/{id}/records
        [HttpPost("{patientId}/records")]
        [Authorize(Roles = $"{Roles.Doctor}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMedicalRecord([FromRoute] int patientId, [FromBody] MedicalRecordRequestDto dto)
        {
            var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _service.AddMedicalRecordAsync(patientId, doctorId, dto);

            if (!response.Success)
            {
                if (response.Message == "Patient not found")
                {
                    return NotFound(response.Message);
                }
                return BadRequest(response.Message);
            }

            return Created(string.Empty, new { recordId = response.RecordId });
        }

        //GET /api/patients/{id}/records
        [HttpGet("{patientId}/records")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMedicalRecords([FromRoute] int patientId)
        {

            int userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!
                );
            try
            {
                var records = await _service.GetPatientRecordsAsync(patientId, userId);
                return Ok(records);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found");
            }
        }
        [HttpPatch("{recordId}/close")]
        [Authorize(Roles = $"{Roles.Doctor}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseMedicalRecord(int recordId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            try
            {
                bool result = await _service.CloseMedicalRecordAsync(recordId, userId);

                if (!result)
                {
                    return BadRequest("Medical record is already closed.");
                }

                return Ok("Medical record closed successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Medical record not found.");
            }
        }

        //PUT /api/v1/MedicalRecord/{recordId}
        [HttpPut("{recordId}")]
        [Authorize(Roles = $"{Roles.Doctor}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMedicalRecord(
            int recordId,
            [FromBody] MedicalRecordRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int doctorId = int.Parse(userIdClaim);

            try
            {
                var result = await _service.UpdateMedicalRecordAsync(recordId, doctorId, dto);

                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok("Medical record updated successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Medical record not found.");
            }
        }
    }
}

