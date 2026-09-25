using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Projects.ArchiveProject;
using OpsFlow.Application.Features.Projects.CreateProject;
using OpsFlow.Application.Features.Projects.GetProject;
using OpsFlow.Application.Features.Projects.ListProjects;
using OpsFlow.Application.Features.Projects.UpdateProject;
using OpsFlow.Contracts.Projects;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public sealed class ProjectsController(
    CreateProjectHandler createProjectHandler,
    ListProjectsHandler listProjectsHandler,
    GetProjectHandler getProjectHandler,
    UpdateProjectHandler updateProjectHandler,
    ArchiveProjectHandler archiveProjectHandler) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.ProjectsCreate)]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createProjectHandler.HandleAsync(
            new CreateProjectCommand(
                request.Name,
                request.Key,
                request.Description),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId = result.ProjectId },
            new ProjectResponse(
                result.ProjectId,
                result.OrganizationId,
                result.Name,
                result.Key,
                result.Description,
                result.Status));
    }

    [HttpGet]
    [Authorize(Policy = PermissionCodes.ProjectsRead)]
    public async Task<ActionResult<IReadOnlyList<ListProjectResponse>>> List(
        CancellationToken cancellationToken)
    {
        var result = await listProjectsHandler.HandleAsync(
            new ListProjectsQuery(),
            cancellationToken);

        return Ok(result.Projects.Select(project => new ListProjectResponse(
            project.Id,
            project.Name,
            project.Key,
            project.Description,
            project.Status)).ToList());
    }

    [HttpGet("{projectId:guid}")]
    [Authorize(Policy = PermissionCodes.ProjectsRead)]
    public async Task<ActionResult<ProjectResponse>> GetById(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await getProjectHandler.HandleAsync(
            new GetProjectQuery(projectId),
            cancellationToken);

        return Ok(new ProjectResponse(
            result.Id,
            result.OrganizationId,
            result.Name,
            result.Key,
            result.Description,
            result.Status));
    }

    [HttpPatch("{projectId:guid}")]
    [Authorize(Policy = PermissionCodes.ProjectsUpdate)]
    public async Task<ActionResult<ProjectResponse>> Update(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateProjectHandler.HandleAsync(
            new UpdateProjectCommand(
                projectId,
                request.Name,
                request.Description),
            cancellationToken);

        return Ok(new ProjectResponse(
            result.Id,
            result.OrganizationId,
            result.Name,
            result.Key,
            result.Description,
            result.Status));
    }

    [HttpPut("{projectId:guid}/archive")]
    [Authorize(Policy = PermissionCodes.ProjectsUpdate)]
    public async Task<IActionResult> Archive(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        await archiveProjectHandler.HandleAsync(
            new ArchiveProjectCommand(projectId),
            cancellationToken);

        return NoContent();
    }
}
