using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Projects;
using OpsFlow.Application.Features.Projects.CreateProject;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public sealed class ProjectsController(
    CreateProjectHandler createProjectHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(
        typeof(CreateProjectResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateProjectResponse>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand(
            request.Name,
            request.Key,
            request.Description);

        var result = await createProjectHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new CreateProjectResponse(
            result.ProjectId,
            result.OrganizationId,
            result.Name,
            result.Key,
            result.Description,
            result.Status);

        return Created(
            $"/api/v1/projects/{result.ProjectId}",
            response);
    }
}
