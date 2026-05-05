using System.Net;
using System.Security.Claims;
using HealthNet.DTOs.AuditDTO;
using HealthNet.Services.AuditService;
using HealthNet.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.ComplianceOfficer}, {Roles.Admin}, {Roles.PublicHealthOfficer}")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;
        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// Endpoint to create a new audit record.
        /// Only accessible by Compliance Officer and Public Health Officer roles.
        /// </summary>
        /// <param name="request"> The DTO containing the audit data </param>
        /// <returns> AuditResponseDto containing the generated AuditId </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAuditAsync([FromBody] CreateAuditDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(AuditHelper.BadRequest);

                // Extract UserId from JWT token — same pattern as ComplianceRecordController
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);

                var result = await _auditService.AddAuditAsync(request, userId);
                return Ok(result); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400 — validation errors
            }
            catch
            {
                return StatusCode(500, AuditHelper.GenericError); // 500 — unexpected error
            }
        }

        /// <summary>
        /// Endpoint to close an audit by setting Status = true (closed).
        /// Only accessible by Compliance Officer and Public Health Officer roles.
        /// </summary>
        /// <param name="id"> The ID of the audit to close </param>
        /// <returns> 200 OK if closed successfully </returns>
        [HttpPatch("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CloseAuditAsync(int id)
        {
            try
            {
                var officerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(officerIdClaim!);

                await _auditService.CloseAuditAsync(id, userId);
                return Ok("Audit closed successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);   // 404 — audit not found
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400 — already closed
            }
            catch
            {
                return StatusCode(500, AuditHelper.GenericError);
            }
        }
    }
}
