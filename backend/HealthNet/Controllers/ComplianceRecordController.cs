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
    [Authorize(Roles = $"{Roles.ComplianceOfficer}, {Roles.Admin}")]
    public class ComplianceRecordController : ControllerBase
    {
        private readonly IComplianceRecordService _complianceRecordService;
        /// <summary>
        /// Constructor for initializing fields
        /// </summary>
        /// <param name="context">This is used to access the database context </param>
        /// <param name="complianceRecordService">This is the service for handling compliance record operations </param>
        public ComplianceRecordController(IComplianceRecordService complianceRecordService)
        {
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

        /// <summary>
        /// Endpoint to search compliance records by filters.Only accessible by users with the "Compliance Officer" role.
        /// </summary>
        /// <param name="filter">The DTO containing optional filter parameters </param>
        /// <returns> Filtered list of compliance records </returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetComplianceRecordsAsync([FromQuery] ComplianceRecordFilterDto filter)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);
                var result = await _complianceRecordService.GetAllComplianceRecordsAsync(filter, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // 400 — no filters / invalid type / invalid result
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);    // 404 — no records found
            }
            catch
            {
                return StatusCode(500, ComplianceHelper.GenericError); // 500 — unexpected error
            }
        }

        /// <summary>
        /// Endpoint to update an existing compliance record. Only accessible by users with the "Compliance Officer" role. Cannot update if record is compliant or deleted. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateComplianceRecordAsync(int id, [FromBody] UpdateComplianceRecordDto request)
        {
            try
            {
                await _complianceRecordService.UpdateComplianceRecordAsync(id, request);
                return Ok("Compliance record updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, ComplianceHelper.GenericError);
            }
        }

        /// <summary>
        /// Endpoint to soft delete a compliance record by setting IsDeleted to true. Only accessible by users with the "Compliance Officer" role. Cannot delete if already deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SoftDeleteComplianceRecordAsync(int id)
        {
            try
            {
                await _complianceRecordService.DeleteComplianceRecordAsync(id);
                return Ok("Compliance record deleted successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, ComplianceHelper.GenericError);
            }
        }

        /// <summary>
        /// Endpoint to get a compliance record by ID. Only accessible by users with the "Compliance Officer" role.
        /// </summary>
        /// <param name="id">The ID of the compliance record</param>
        /// <returns>The compliance record matching the given ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetComplianceRecordByIdAsync(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaim!);
                var result = await _complianceRecordService.GetComplianceRecordByIdAsync(id, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, ComplianceHelper.GenericError);
            }
        }
    }
}
