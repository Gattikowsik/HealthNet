using System.Security.Claims;
using HealthNet.DTOs;
using HealthNet.Services;
using HealthNet.Utility;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HealthNet.Controllers
{
    [Route("api/v1/[Controller]")]
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

        // <summary>
        // GetMyReports — gets all symptoms repports submitted by the authenticated citizen
        // </summary>
        // <param name="request"> SymptomReportResponse DTO for data transfer from client </param>
        // Citizen ONLY – View ONLY his own reports
        [HttpGet("mine")]
        [Authorize(Roles = "Citizen")]
        public async Task<IActionResult> GetMyReports(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetMineAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        // <summary>
        // GetAllReports — gets all symptoms reports in the system (for Doctor/Researcher/Admin)
        // </summary>
        // <param name="request"> SymptomReportResponse DTO for data transfer from client </param>
        //Doctor / Researcher / Admin – View ALL symptom reports
        [HttpGet]
        [Authorize(Roles = "Doctor,Researcher,Admin")]
        public async Task<IActionResult> GetAllReports(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetAllAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}