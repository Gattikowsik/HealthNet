using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthNet.Services.PatientServices;
using HealthNet.DTOs.PateintDto;
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
    }
}
