using System.Net;
using HealthNet.DTOs.ReportingAndAnalyticsDTO;
using HealthNet.Services.ReportingAndAnalyticsServices;
using HealthNet.Utility;
using HealthNetDb.Entities;
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
        [HttpGet("outbreaks")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOutbreakAnalytics([FromQuery] OutbreakAnalyticsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.OutbreakAnalyticsReportService(request);
            if (!response.Success)
            {
                return BadRequest(response?.Message);
            }

            return Ok(response);
        }

        // <summary>
        // GetPatientAnalytics — Get Patients for given Query Filters
        // </summary>
        // <param name="request"> PatientAnalyticsReportRequest DTO for data transfer from client </param>
        [HttpGet("patients")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPatientAnalytics([FromQuery] PatientAnalyticsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.PatientAnalyticsReportService(request);
            if (!response.Success)
            {
                return BadRequest(response?.Message);
            }

            return Ok(response);
        }

        // <summary>
        // GetComplianceRecords — Get Compliance Records for given Query Filters
        // </summary>
        // <param name="request"> ComplianceMetricsReportRequest DTO for data transfer from client </param>
        [Authorize(Roles = $"{Roles.Admin}, {Roles.ComplianceOfficer}")]
        [HttpGet("compliance")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetComplianceRecords([FromQuery] ComplianceMetricsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.ComplianceMetricsReportService(request);

            if (!response.Success)
            {
                return BadRequest(response?.Message);
            }

            return Ok(response);
        }

        // <summary>
        // GetEpidemiologyTrendRecords — Get Epidemiology Trends Records for given Query Filters
        // </summary>
        // <param name="request"> EpidemiologicalAnalyticsReportRequest DTO for data transfer from client </param>
        [HttpGet("epidemiology")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetEpidemiologyTrendRecords([FromQuery] EpidemiologicalAnalyticsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.EpidemiologicalReportService(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        // <summary>
        // GetCaseReportRecords — Get Case Reports Records for given Query Filters
        // </summary>
        // <param name="request"> CaseAnalyticsReportRequest DTO for data transfer from client </param>
        [HttpGet("cases")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCaseReportRecords([FromQuery] CaseAnalyticsReportRequest request)
        {
            var response = await _reportingAndAnalyticsService.CaseAnalyticsReportService(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
    }
}
