using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Teams.AddTeamMember;
using OpsFlow.Application.Features.Teams.ArchiveTeam;
using OpsFlow.Application.Features.Teams.ChangeTeamLead;
using OpsFlow.Application.Features.Teams.CreateTeam;
using OpsFlow.Application.Features.Teams.GetTeam;
using OpsFlow.Application.Features.Teams.ListTeams;
using OpsFlow.Application.Features.Teams.RemoveTeamMember;
using OpsFlow.Application.Features.Teams.UpdateTeam;
using OpsFlow.Contracts.Teams;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/teams")]
[Authorize]
public sealed class TeamsController(
    CreateTeamHandler createTeamHandler,
    ListTeamsHandler listTeamsHandler,
    GetTeamHandler getTeamHandler,
    UpdateTeamHandler updateTeamHandler,
    ArchiveTeamHandler archiveTeamHandler,
    AddTeamMemberHandler addTeamMemberHandler,
    RemoveTeamMemberHandler removeTeamMemberHandler,
    ChangeTeamLeadHandler changeTeamLeadHandler) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.TeamsCreate)]
    public async Task<ActionResult<CreateTeamResult>> Create(
        CreateTeamRequest request,
        CancellationToken cancellationToken)
        => Ok(await createTeamHandler.Handle(
            new CreateTeamCommand(request.Name, request.Description),
            cancellationToken));

    [HttpGet]
    [Authorize(Policy = PermissionCodes.TeamsRead)]
    public async Task<ActionResult<ListTeamsResult>> List(
        CancellationToken cancellationToken)
        => Ok(await listTeamsHandler.Handle(new ListTeamsQuery(), cancellationToken));

    [HttpGet("{teamId:guid}")]
    [Authorize(Policy = PermissionCodes.TeamsRead)]
    public async Task<ActionResult<GetTeamResult>> Get(
        Guid teamId,
        CancellationToken cancellationToken)
        => Ok(await getTeamHandler.Handle(
            new GetTeamQuery(teamId),
            cancellationToken));

    [HttpPatch("{teamId:guid}")]
    [Authorize(Policy = PermissionCodes.TeamsUpdate)]
    public async Task<ActionResult<UpdateTeamResult>> Update(
        Guid teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken)
        => Ok(await updateTeamHandler.Handle(
            new UpdateTeamCommand(teamId, request.Name, request.Description),
            cancellationToken));

    [HttpPut("{teamId:guid}/archive")]
    [Authorize(Policy = PermissionCodes.TeamsUpdate)]
    public async Task<IActionResult> Archive(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        await archiveTeamHandler.Handle(
            new ArchiveTeamCommand(teamId),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{teamId:guid}/members")]
    [Authorize(Policy = PermissionCodes.TeamsManageMembers)]
    public async Task<ActionResult<AddTeamMemberResult>> AddMember(
        Guid teamId,
        AddTeamMemberRequest request,
        CancellationToken cancellationToken)
        => Ok(await addTeamMemberHandler.Handle(
            new AddTeamMemberCommand(teamId, request.MembershipId),
            cancellationToken));

    [HttpDelete("{teamId:guid}/members/{membershipId:guid}")]
    [Authorize(Policy = PermissionCodes.TeamsManageMembers)]
    public async Task<IActionResult> RemoveMember(
        Guid teamId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        await removeTeamMemberHandler.Handle(
            new RemoveTeamMemberCommand(teamId, membershipId),
            cancellationToken);

        return NoContent();
    }

    [HttpPut("{teamId:guid}/lead")]
    [Authorize(Policy = PermissionCodes.TeamsManageMembers)]
    public async Task<IActionResult> ChangeLead(
        Guid teamId,
        ChangeTeamLeadRequest request,
        CancellationToken cancellationToken)
    {
        await changeTeamLeadHandler.Handle(
            new ChangeTeamLeadCommand(teamId, request.MembershipId),
            cancellationToken);

        return NoContent();
    }
}
