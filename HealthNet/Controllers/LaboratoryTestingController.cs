using System;
using System.Net;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Services.LabTestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthNetDb.Entities;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Lab Technician")]
    public class LaboratoryTestingController : ControllerBase
    {
        private readonly ILaboratoryTestingService _laboratoryTestingService;

        // <summary>
        // Constructor for initializing fields
        // </summary>
        // <param name="laboratoryTestingService"> laboratoryTestingService object to use the methods in it. </param>
        public LaboratoryTestingController(ILaboratoryTestingService laboratoryTestingService)
        {
            _laboratoryTestingService = laboratoryTestingService;
        }

        // <summary>
        // CreateLabTestAsync — creates a new lab test for a patient
        // </summary>
        // <param name="request"> LaboratoryTestingRequest DTO for data transfer from client </param>
        [HttpPost("lab-tests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateLabTestAsync([FromBody] LaboratoryTestingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);

                var result = await _laboratoryTestingService.CreateLaboratoryTestAsync(request, userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Patient with ID {request.PatientId} not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lab test created successfully.",
                    data = result
                });
            }
            catch (HealthNetException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}