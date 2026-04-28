using HealthNet.Services.MedicalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthNet.DTOs.MedicalRecordDto;
using Microsoft.AspNetCore.Authorization;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]/patients/{id}/records")]
    [Authorize(Roles = "Doctor")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
        {
            _service = service;
        }
        // ✅ POST /api/patients/{id}/records
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMedicalRecord(int id, [FromBody] MedicalRecordRequestDto dto)
        {

            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(doctorIdClaim))
                return Unauthorized();

            int doctorId = int.Parse(doctorIdClaim);
            try
            {
                var response = await _service
                    .AddMedicalRecordAsync(id, doctorId, dto);

                if (response.Success == false)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = response.Message
                    });
                }
                return Created(string.Empty, new
                {
                    recordId = response.RecordId
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found");
            }
        }
    }
}
