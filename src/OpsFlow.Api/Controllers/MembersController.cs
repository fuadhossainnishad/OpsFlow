using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Members;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Members.AcceptInvitation;
using OpsFlow.Application.Features.Members.ChangeMemberRole;
using OpsFlow.Application.Features.Members.DeactivateMember;
using OpsFlow.Application.Features.Members.InviteMember;
using OpsFlow.Application.Features.Members.ListMembers;
using OpsFlow.Application.Features.Members.ReactivateMember;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/members")]
[Authorize]
public sealed class MembersController(
    InviteMemberHandler inviteMemberHandler,
    AcceptInvitationHandler acceptInvitationHandler,
    ListMembersHandler listMembersHandler,
    ChangeMemberRoleHandler changeMemberRoleHandler,
    DeactivateMemberHandler deactivateMemberHandler,
    ReactivateMemberHandler reactivateMemberHandler)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionCodes.MembersRead)]
    public async Task<ActionResult<ListMembersResult>> List(
        CancellationToken cancellationToken)
    {
        var result = await listMembersHandler.HandleAsync(
            new ListMembersQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("invitations")]
    [Authorize(Policy = PermissionCodes.MembersInvite)]
    public async Task<ActionResult<InviteMemberResponse>> Invite(
        InviteMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await inviteMemberHandler.HandleAsync(
            new InviteMemberCommand(
                request.Email,
                request.RoleName),
            cancellationToken);

        return Ok(new InviteMemberResponse(
            result.InvitationId,
            result.OrganizationId,
            result.Email,
            result.RoleName,
            result.ExpiresAtUtc,
            result.Token));
    }

    [HttpPost("invitations/accept")]
    public async Task<ActionResult<AcceptInvitationResponse>> Accept(
        AcceptInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await acceptInvitationHandler.HandleAsync(
            new AcceptInvitationCommand(request.Token),
            cancellationToken);

        return Ok(new AcceptInvitationResponse(
            result.MembershipId,
            result.OrganizationId,
            result.UserId,
            result.RoleId));
    }

    [HttpPut("{membershipId:guid}/role")]
    [Authorize(Policy = PermissionCodes.MembersManage)]
    public async Task<ActionResult<ChangeMemberRoleResponse>> ChangeRole(
        Guid membershipId,
        ChangeMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await changeMemberRoleHandler.HandleAsync(
            new ChangeMemberRoleCommand(
                membershipId,
                request.RoleName),
            cancellationToken);

        return Ok(new ChangeMemberRoleResponse(
            result.MembershipId,
            result.OrganizationId,
            result.UserId,
            result.RoleId,
            result.RoleName,
            result.IsActive));
    }

    [HttpPut("{membershipId:guid}/deactivate")]
    [Authorize(Policy = PermissionCodes.MembersManage)]
    public async Task<ActionResult<MemberStatusResponse>> Deactivate(
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        var result = await deactivateMemberHandler.HandleAsync(
            new DeactivateMemberCommand(membershipId),
            cancellationToken);

        return Ok(new MemberStatusResponse(
            result.MembershipId,
            result.OrganizationId,
            result.UserId,
            result.IsActive));
    }

    [HttpPut("{membershipId:guid}/reactivate")]
    [Authorize(Policy = PermissionCodes.MembersManage)]
    public async Task<ActionResult<MemberStatusResponse>> Reactivate(
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        var result = await reactivateMemberHandler.HandleAsync(
            new ReactivateMemberCommand(membershipId),
            cancellationToken);

        return Ok(new MemberStatusResponse(
            result.MembershipId,
            result.OrganizationId,
            result.UserId,
            result.IsActive));
    }
}
