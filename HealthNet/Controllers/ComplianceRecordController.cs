using System.Net;
using HealthNet.DTOs.ComplianceRecordDto;
using HealthNet.Services.ComplianceRecordServices;
using HealthNetDb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthNet.Utility;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Compliance Officer")]
    public class ComplianceRecordController : ControllerBase
    {
        private readonly HealthNetContext _context;
        private readonly IComplianceRecordService _complianceRecordService;
        /// <summary>
        /// Constructor for initializing fields
        /// </summary>
        /// <param name="context">This is used to access the database context </param>
        /// <param name="complianceRecordService">This is the service for handling compliance record operations </param>
        public ComplianceRecordController(HealthNetContext context, IComplianceRecordService complianceRecordService)
        {
            _context = context;
            _complianceRecordService = complianceRecordService;
        }

        /// <summary>
        /// Endpoint to add a new compliance record. Only accessible by users with the "Compliance Officer" role.
        /// </summary>
        /// <param name="request">The DTO containing the compliance record data </param>
        /// <returns>The response DTO containing the created compliance record  </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddComplianceRecordAsync([FromBody] CreateComplianceRecordDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);
                if (request == null)
                    return BadRequest(ComplianceHelper.BadRequest);

                // Pass userId separately to the service
                var result = await _complianceRecordService.AddComplianceRecordAsync(request, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, ComplianceHelper.GenericError);
            }
        }
    }
}
