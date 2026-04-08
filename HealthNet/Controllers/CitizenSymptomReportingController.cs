using System.Security.Claims;
using HealthNet.DTOs;
using HealthNet.Services;
using HealthNet.Utility;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HealthNet.Controllers
{
    [Route("api/v1/Citizen Symptom Reporting")]
    [ApiController]
    [ProducesResponseType(typeof(SubmitSymptomReportResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CitizenSymptomReportingController : ControllerBase
    {
        private readonly ISubmitSymptomReportService _service;
        public CitizenSymptomReportingController(ISubmitSymptomReportService service)
        {
            _service = service;
        }
        [HttpPost]
        [Authorize(Roles = "Citizen")]
        public async Task<IActionResult> Submit([FromBody] SubmitSymptomReportRequestDto request)
        {
            // Model validation (Required fields etc.)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JSON validation using Utility
            if (!SymptomReportHelper.IsValidJson(request.SymptomsJson))
            {
                ModelState.AddModelError(nameof(request.SymptomsJson),
                    "SymptomsJson must be a valid JSON string");
                return BadRequest(ModelState);              // 400 with details
            }

            //Get citizenId from JWT
            var citizenId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            //Call service
            var response = await _service.SubmitAsync(request, citizenId);

            // Return 201
            return Created($"/api/v1/Citizen Symptom Reporting/{response.ReportId}", response);
        }
    }
}