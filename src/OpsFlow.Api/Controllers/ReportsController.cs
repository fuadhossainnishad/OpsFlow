using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Reports.GetDashboardSummary;
using OpsFlow.Application.Features.Reports.GetProjectReport;
using OpsFlow.Application.Features.Reports.GetTimeReport;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public sealed class ReportsController(
    GetDashboardSummaryHandler dashboardHandler,
    GetTimeReportHandler timeReportHandler,
    GetProjectReportHandler projectReportHandler) : ControllerBase
{
    [HttpGet("dashboard")]
    [Authorize(Policy = PermissionCodes.ReportsRead)]
    public async Task<ActionResult<DashboardSummaryResult>> GetDashboard(
        CancellationToken cancellationToken)
    {
        return Ok(await dashboardHandler.HandleAsync(cancellationToken));
    }

    [HttpGet("time")]
    [Authorize(Policy = PermissionCodes.ReportsRead)]
    public async Task<ActionResult<IReadOnlyList<TimeReportRow>>> GetTimeReport(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        return Ok(await timeReportHandler.HandleAsync(
            from,
            to,
            cancellationToken));
    }

    [HttpGet("projects")]
    [Authorize(Policy = PermissionCodes.ReportsRead)]
    public async Task<ActionResult<IReadOnlyList<ProjectReportRow>>> GetProjectReport(
        CancellationToken cancellationToken)
    {
        return Ok(await projectReportHandler.HandleAsync(cancellationToken));
    }
}
