using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.TimeTracking;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.TimeTracking.CreateTimeEntry;
using OpsFlow.Application.Features.TimeTracking.DeleteTimeEntry;
using OpsFlow.Application.Features.TimeTracking.GetTimeEntry;
using OpsFlow.Application.Features.TimeTracking.ListTimeEntries;
using OpsFlow.Application.Features.TimeTracking.UpdateTimeEntry;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/time-entries")]
[Authorize]
public sealed class TimeEntriesController(
    CreateTimeEntryHandler createTimeEntryHandler,
    ListTimeEntriesHandler listTimeEntriesHandler,
    GetTimeEntryHandler getTimeEntryHandler,
    UpdateTimeEntryHandler updateTimeEntryHandler,
    DeleteTimeEntryHandler deleteTimeEntryHandler)
    : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.TimeEntriesCreate)]
    public async Task<ActionResult<CreateTimeEntryResult>> Create(
        CreateTimeEntryRequest request,
        CancellationToken cancellationToken)
        => Ok(await createTimeEntryHandler.HandleAsync(
            new CreateTimeEntryCommand(
                request.ProjectId,
                request.TaskId,
                request.Description,
                request.StartedAtUtc,
                request.EndedAtUtc),
            cancellationToken));

    [HttpGet]
    [Authorize(Policy = PermissionCodes.TimeEntriesRead)]
    public async Task<ActionResult<ListTimeEntriesResult>> List(
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? taskId,
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        CancellationToken cancellationToken)
        => Ok(await listTimeEntriesHandler.HandleAsync(
            new ListTimeEntriesQuery(
                projectId,
                taskId,
                fromUtc,
                toUtc),
            cancellationToken));

    [HttpGet("{timeEntryId:guid}")]
    [Authorize(Policy = PermissionCodes.TimeEntriesRead)]
    public async Task<ActionResult<GetTimeEntryResult>> Get(
        Guid timeEntryId,
        CancellationToken cancellationToken)
        => Ok(await getTimeEntryHandler.HandleAsync(
            new GetTimeEntryQuery(timeEntryId),
            cancellationToken));

    [HttpPatch("{timeEntryId:guid}")]
    [Authorize(Policy = PermissionCodes.TimeEntriesUpdate)]
    public async Task<ActionResult<UpdateTimeEntryResult>> Update(
        Guid timeEntryId,
        UpdateTimeEntryRequest request,
        CancellationToken cancellationToken)
        => Ok(await updateTimeEntryHandler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                request.TaskId,
                request.Description,
                request.StartedAtUtc,
                request.EndedAtUtc),
            cancellationToken));

    [HttpDelete("{timeEntryId:guid}")]
    [Authorize(Policy = PermissionCodes.TimeEntriesDelete)]
    public async Task<IActionResult> Delete(
        Guid timeEntryId,
        CancellationToken cancellationToken)
    {
        await deleteTimeEntryHandler.HandleAsync(
            new DeleteTimeEntryCommand(timeEntryId),
            cancellationToken);

        return NoContent();
    }
}
