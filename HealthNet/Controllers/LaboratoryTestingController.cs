using System;
using System.Net;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Services.LabTestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateLabTestAsync([FromBody] LaboratoryTestingRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim!);
            // Call service to create lab test
            var result = await _laboratoryTestingService.CreateLaboratoryTestAsync(request, userId);

            if (result == null)
            {
                // Patient not found, return 404
                return NotFound(new
                {
                    success = false,
                    message = $"Patient with ID {request.PatientId} not found."
                });
            }

            // Return success response with created lab test details
            return Ok(new
            {
                success = true,
                message = "Lab test created successfully.",
                data = result
            });
        }
    }
}