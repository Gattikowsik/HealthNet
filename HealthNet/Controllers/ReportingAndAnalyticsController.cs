using System.Net;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;
using HealthNet.Services.ReportingAndAnalyticsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthNet.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportingAndAnalyticsController : ControllerBase
    {
        private readonly IReportingAndAnalyticsService _reportingAndAnalyticsService;
        public ReportingAndAnalyticsController(IReportingAndAnalyticsService reportingAndAnalyticsService)
        {
            _reportingAndAnalyticsService = reportingAndAnalyticsService;
        }

        // <summary>
        // GetOutbreakAnalytics — Get Outbreaks for given Query Filters
        // </summary>
        // <param name="request"> OutbreakAnalyticsReportRequest DTO for data transfer from client </param>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOutbreakAnalytics([FromQuery]OutbreakAnalyticsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.OutbreakAnalyticsReportService(request);
            if (!response.Success)
            {
                return BadRequest(response?.Message);
            }

            return Ok(response);
        }
    }
}
