using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthNet.Services.PatientServices;
using HealthNet.DTOs.PatientDto;
using Microsoft.AspNetCore.Authorization;
using HealthNetDb.Entities;
using System.Security.Claims;

namespace HealthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientManagementController : ControllerBase
    {
         private readonly IPatientManagementService _patientService;

    public PatientManagementController(IPatientManagementService patientService)
    {
        _patientService = patientService;
    }

    // ✅ GET /api/patients?name=John&status=true&pageNumber=1&pageSize=10
    [HttpGet]
    [Authorize]
   // [Authorize(Roles="Doctor")]
    public async Task<IActionResult> SearchPatients([FromQuery] PatientSearchDto searchDto)
    {

        var result = await _patientService.SearchPatientsAsync(searchDto);
        return Ok(result);
    }
    
   [HttpPost]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientRequestDto dto)
    {
        //✅ Required fields missing → 400
       if (!ModelState.IsValid)
           return BadRequest(ModelState);

        var response = await _patientService.RegisterPatientAsync(dto);

        // ✅ Return 201 with PatientId
        return Created(
            $"/api/patients/{response.PatientId}",
            response);
    }
    }
}
