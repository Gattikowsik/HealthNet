using System.Security.Claims;
using HealthNet.DTOs;
using HealthNet.DTOs.CitizenSymptomReportingDTO;
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
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Citizen}")]
        public async Task<IActionResult> Submit(
    [FromBody] SubmitSymptomReportRequestDto request)
        {
            try
            {
                var citizenId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var response = await _service.SubmitAsync(request, citizenId);

                return Created(
                    $"/api/v1/CitizenSymptomReporting/{response.ReportId}",
                    response);
            }
            catch (HealthNetException ex) // MUST be this exact type
            {
                var problem = new ProblemDetails
                {
                    Title = "Invalid request payload",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7807"
                };

                return BadRequest(problem);
            }
        }

        // <summary>
        // GetMyReports — gets all symptoms repports submitted by the authenticated citizen
        // </summary>
        // <param name="request"> SymptomReportResponse DTO for data transfer from client </param>
        // Citizen ONLY – View ONLY his own reports
        [HttpGet("mine")]
        [Authorize(Roles = $"{Roles.Citizen}")]
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
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}, {Roles.Researcher}")]
        public async Task<IActionResult> GetAllReports(
        [FromQuery] int? citizenId,
        [FromQuery] DateTime? reportDate,
        [FromQuery] SymptomStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _service.GetAllAsync(
                    userId, citizenId, reportDate, status, pageNumber, pageSize);

                return Ok(result);
            }
            catch (HealthNetException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
        // <summary>
        // UpdateStatusAsync — updates the status for the symptom report (for Doctor/Public Health Officer/Admin)
        // </summary>
        // <param name="request"> UpdateSymptomStatusRequest DTO for data transfer from client </param>
        //Doctor / Public Health Officer / Admin – View ALL symptom reports
        [HttpPatch("{id}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}, {Roles.PublicHealthOfficer}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateSymptomStatusRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var updated = await _service.UpdateStatusAsync(id, request.Status, userId);

                if (!updated)
                    return NotFound("Symptom report not found.");

                return Ok("Status updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // "Invalid status value"
            }
        }
        // <summary>
        // DeleteReportAsync — softdelete the status for the symptom report (for Doctor/Public Health Officer/Admin)
        // </summary>
        // <param name="request"> DeleteSymptomStatusRequest DTO for data transfer from client </param>
        //Doctor / Public Health Officer / Admin – Delete symptom reports
        [HttpDelete("{reportId}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Doctor}, {Roles.PublicHealthOfficer}")]
        public async Task<IActionResult> DeleteReport(int reportId)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var deleted = await _service.SoftDeleteAsync(reportId, userId);

            if (!deleted)
                return NotFound(new { message = "Symptom report not found." });

            return Ok(new { message = "Symptom report deleted successfully." });
        }
    }
}