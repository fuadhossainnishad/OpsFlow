using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Features.Tasks.CreateTask;
using OpsFlow.Application.Features.Tasks.GetTask;
using OpsFlow.Api.Contracts.Tasks;
using OpsFlow.Application.Authorization;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[Authorize]
public sealed class TasksController(
    CreateTaskHandler createTaskHandler,
    GetTaskHandler getTaskHandler) : ControllerBase
{
    [Authorize(Policy = PermissionCodes.TasksCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateTaskResponse>> Create(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.ProjectId,
            request.Title,
            request.Description,
            request.AssigneeUserId);

        var result = await createTaskHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new CreateTaskResponse(
            result.TaskId,
            result.OrganizationId,
            result.ProjectId,
            result.Title,
            result.Description,
            result.AssigneeUserId,
            result.Status);

        return CreatedAtAction(
            nameof(Get),
            new { taskId = result.TaskId },
            response);
    }
    [Authorize(Policy = PermissionCodes.TasksRead)]
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(GetTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTaskResponse>> Get(
            Guid taskId,
            CancellationToken cancellationToken)
    {
        var result = await getTaskHandler.HandleAsync(
            new GetTaskQuery(taskId),
            cancellationToken);

        var response = new GetTaskResponse(
            result.TaskId,
            result.OrganizationId,
            result.ProjectId,
            result.Title,
            result.Description,
            result.AssigneeUserId,
            result.Status,
            result.CreatedAtUtc,
            result.UpdatedAtUtc);

        return Ok(response);
    }
}
