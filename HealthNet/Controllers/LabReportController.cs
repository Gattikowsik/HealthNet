using System.Net;
using System.Security.Claims;
using HealthNet.DTOs.LabReportDTO;
using HealthNet.Services.LabReportServices;
using HealthNet.Utility;
using HealthNetDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LabReportController : ControllerBase
    {
        private readonly ILabReportService _labReportService;

        // <summary>
        // Constructor for initializing fields
        // </summary>
        // <param name="labReportService"> labReportService object to use the methods in it. </param>
        // <param name="webHostEnvironment"> webHostEnvironment object to get the wwwroot path for file storage. </param>
        public LabReportController(ILabReportService labReportService)
        {
            _labReportService = labReportService;
        }

        // <summary>
        // UploadLabReportAsync — uploads a lab report for a completed lab test
        // </summary>
        // <param name="request"> LabReportRequest DTO for data transfer from client </param>
        [HttpPost]
        [Authorize(Roles = Roles.LabTechnician)]
        [Consumes("multipart/form-data")] // Specify that the endpoint accepts multipart/form-data for file uploads
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UploadLabReportAsync([FromForm] LabReportRequest request)
        {
            try
            {
                // Extract userId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);

                // Call the service method to upload the lab report
                var result = await _labReportService.UploadLabReportAsync(request, userId);

                return Ok(new
                {
                    success = true,
                    message = "Lab report uploaded successfully.",
                    data = result
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
