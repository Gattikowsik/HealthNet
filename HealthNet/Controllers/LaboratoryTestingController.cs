using System;
using System.Net;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Services.LabTestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthNetDb.Entities;
using HealthNet.Utility;

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

        /// <summary>
        /// GetLabTestsAsync — retrieves lab tests based on optional filters (Type, Status, Date).
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>This will return a list of lab tests based on the provided filters.</returns>
        [HttpGet("lab-tests")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLabTestsAsync([FromQuery] LaboratoryTestingFilterRequest filter)
        {
            try
            {
                // Extract userId from JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);
                var result = await _laboratoryTestingService.GetLabTestsAsync(filter, userId);
                return Ok(new
                {
                    success = true,
                    count = result.Count(),
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

        /// <summary>
        /// UpdateLabTestAsync — updates an existing lab test
        /// Doctor can update Type and TechnicianId
        /// Assigned Lab Tech can update Type only
        /// </summary>
        [HttpPut("{testId}")]
        [Authorize(Roles = $"{Roles.Doctor},{Roles.LabTechnician}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateLabTestAsync(int testId, [FromBody] LaboratoryTestingUpdateRequest request)
        {
            try
            {
                // Validate testId
                if (testId <= 0)
                {
                    return BadRequest(new { success = false, message = LabTestHelper.InvalidTestIdMessage });
                }

                // Extract userId and role from JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId      = int.Parse(userIdClaim!);
                var userRole = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value!;

                var result = await _laboratoryTestingService.UpdateLabTestAsync(testId, request, userId, userRole);

                return Ok(new
                {
                    success = true,
                    message = "Lab test updated successfully.",
                    data    = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
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