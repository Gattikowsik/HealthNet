using System;
using System.Net;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Services.LabTestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/lab-tests")]
    [ApiController]
    [Authorize(Roles = "Doctor,Lab Technician")]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        // <summary>
        // Constructor for initializing fields
        // </summary>
        // <param name="labTestService"> labTestService object to use the methods in it. </param>
        public LabTestController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        // <summary>
        // CreateLabTestAsync — creates a new lab test for a patient
        // </summary>
        // <param name="request"> LabTestRequest DTO for data transfer from client </param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateLabTestAsync([FromBody] LabTestRequest request)
        {
            // Call service to create lab test
            var result = await _labTestService.CreateLabTestAsync(request);

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