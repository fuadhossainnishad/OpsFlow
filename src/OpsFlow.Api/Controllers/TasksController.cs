using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Tasks;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Tasks.AssignTask;
using OpsFlow.Application.Features.Tasks.ChangeTaskStatus;
using OpsFlow.Application.Features.Tasks.CreateTask;
using OpsFlow.Application.Features.Tasks.GetTask;
using OpsFlow.Application.Features.Tasks.UpdateTask;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[Authorize]
public sealed class TasksController(
    CreateTaskHandler createTaskHandler,
    GetTaskHandler getTaskHandler,
    UpdateTaskHandler updateTaskHandler,
    AssignTaskHandler assignTaskHandler,
    ChangeTaskStatusHandler changeTaskStatusHandler) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.TasksCreate)]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateTaskResponse>> Create(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createTaskHandler.HandleAsync(
            new CreateTaskCommand(
                request.ProjectId,
                request.Title,
                request.Description,
                request.AssigneeUserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new { taskId = result.TaskId },
            new CreateTaskResponse(
                result.TaskId,
                result.OrganizationId,
                result.ProjectId,
                result.Title,
                result.Description,
                result.AssigneeUserId,
                result.Status,
                result.RowVersion));
    }

    [HttpGet("{taskId:guid}")]
    [Authorize(Policy = PermissionCodes.TasksRead)]
    [ProducesResponseType(typeof(GetTaskResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetTaskResponse>> Get(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var result = await getTaskHandler.HandleAsync(
            new GetTaskQuery(taskId),
            cancellationToken);

        return Ok(new GetTaskResponse(
            result.TaskId,
            result.OrganizationId,
            result.ProjectId,
            result.Title,
            result.Description,
            result.AssigneeUserId,
            result.Status,
            result.CreatedAtUtc,
            result.UpdatedAtUtc,
            result.RowVersion));
    }

    [HttpPatch("{taskId:guid}")]
    [Authorize(Policy = PermissionCodes.TasksUpdate)]
    [ProducesResponseType(typeof(UpdateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateTaskResponse>> Update(
        Guid taskId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateTaskHandler.HandleAsync(
            new UpdateTaskCommand(
                taskId,
                request.Title,
                request.Description,
                request.RowVersion),
            cancellationToken);

        return Ok(new UpdateTaskResponse(
            result.TaskId,
            result.OrganizationId,
            result.ProjectId,
            result.Title,
            result.Description,
            result.AssigneeUserId,
            result.Status,
            result.CreatedAtUtc,
            result.UpdatedAtUtc,
            result.RowVersion));
    }

    [HttpPut("{taskId:guid}/assignee")]
    [Authorize(Policy = PermissionCodes.TasksAssign)]
    [ProducesResponseType(typeof(AssignTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AssignTaskResponse>> Assign(
        Guid taskId,
        AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await assignTaskHandler.HandleAsync(
            new AssignTaskCommand(
                taskId,
                request.AssigneeUserId,
                request.RowVersion),
            cancellationToken);

        return Ok(new AssignTaskResponse(
            result.TaskId,
            result.OrganizationId,
            result.AssigneeUserId,
            result.Status,
            result.RowVersion));
    }

    [HttpPut("{taskId:guid}/status")]
    [Authorize(Policy = PermissionCodes.TasksUpdate)]
    [ProducesResponseType(typeof(ChangeTaskStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChangeTaskStatusResponse>> ChangeStatus(
        Guid taskId,
        ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await changeTaskStatusHandler.HandleAsync(
            new ChangeTaskStatusCommand(
                taskId,
                request.Status,
                request.RowVersion),
            cancellationToken);

        return Ok(new ChangeTaskStatusResponse(
            result.TaskId,
            result.OrganizationId,
            result.Status,
            result.RowVersion));
    }
}
