using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthNet.Services;
using HealthNet.DTOs.CaseDto;
using System.Net;   
using HealthNet.Utility;
using Microsoft.AspNetCore.Authorization;
using HealthNet.Services.CaseService;
using System.Security.Claims;
namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.PublicHealthOfficer}, {Roles.Admin}")]
    public class CasesController : ControllerBase
    {
        private readonly ICasesService _casesService;

        public CasesController(ICasesService casesService)
        {
            _casesService = casesService;
        }

        /// <summary>
        /// Doctor creates a case for a symptom report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCaseAsync([FromBody] CreateCaseDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(CasesHelper.BadRequest);

                var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int doctorId = int.Parse(doctorIdClaim!);

                var result = await _casesService.CreateCaseAsync(request, doctorId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, CasesHelper.GenericError);
            }
        }

        /// <summary>
        /// Get all cases
        /// </summary>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllCasesAsync()
        {
            try
            {
                var result = await _casesService.GetAllCasesAsync();
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, CasesHelper.GenericError);
            }
        }

        /// <summary>
        /// Get a case by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCaseByIdAsync(int id)
        {
            try
            {
                var result = await _casesService.GetCaseByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, CasesHelper.GenericError);
            }
        }

        /// <summary>
        /// Doctor updates only the diagnosis
        /// Only allowed when Status = false (Under Treatment)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateCaseDiagnosisAsync(int id, [FromBody] UpdateCaseDiagnosisDto request)
        {
            try
            {
                var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int doctorId = int.Parse(doctorIdClaim!);

                await _casesService.UpdateCaseDiagnosisAsync(id, request, doctorId);
                return Ok("Case diagnosis updated successfully.");
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
                return StatusCode(500, CasesHelper.GenericError);
            }
        }

        /// <summary>
        /// Doctor deletes a case
        /// System sets Status = true (Recovered) then deletes
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteCaseAsync(int id)
        {
            try
            {
                var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int doctorId = int.Parse(doctorIdClaim!);

                await _casesService.DeleteCaseAsync(id, doctorId);
                return Ok("Case marked as recovered and deleted successfully.");
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
                return StatusCode(500, CasesHelper.GenericError);
            }
        }
    }
}
