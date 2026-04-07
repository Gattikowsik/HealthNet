using System.Security.Claims;
using HealthNet.DTOs;
using HealthNet.Services;
using HealthNet.Utility;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HealthNet.Controllers
{
    [Route("api/v1/symptomReport")]
    [ApiController]
    [Authorize(Roles = "Citizen")]
    public class SymptomReportController : ControllerBase
    {    
        private readonly ISymptomReportService _service;
        public SymptomReportController(ISymptomReportService service)
        {
            _service = service;
        }
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SymptomReportRequestDto request)
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
            return Created( $"/api/symptom-reports/{response.ReportId}",response);
        }
    }
}