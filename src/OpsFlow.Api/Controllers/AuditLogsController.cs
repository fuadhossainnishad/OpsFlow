using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Auditing.ListAuditLogs;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/audit-logs")]
[Authorize]
public sealed class AuditLogsController(
    ListAuditLogsHandler listAuditLogsHandler) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionCodes.MembersRead)]
    public async Task<ActionResult<ListAuditLogsResult>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? resource = null,
        [FromQuery] Guid? resourceId = null,
        CancellationToken cancellationToken = default)
        => Ok(await listAuditLogsHandler.HandleAsync(
            new ListAuditLogsQuery(
                page,
                pageSize,
                resource,
                resourceId),
            cancellationToken));
}
