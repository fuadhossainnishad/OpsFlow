using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.RemoveTeamMember;

public sealed class RemoveTeamMemberHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<RemoveTeamMemberResult> Handle(
        RemoveTeamMemberCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var team = await teamRepository.GetByIdAsync(
            organizationId,
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            throw new NotFoundException("Team was not found.");
        }

        var member = await teamRepository.GetTeamMemberAsync(
            organizationId,
            command.TeamId,
            command.MembershipId,
            cancellationToken);

        if (member is null)
        {
            throw new NotFoundException("Team member was not found.");
        }

        if (team.TeamLeadMembershipId == command.MembershipId)
        {
            throw new ConflictException(
                "The current team lead cannot be removed until another lead is assigned.");
        }

        teamRepository.RemoveMember(member);

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.member_removed",
            "team",
            team.Id,
            JsonSerializer.Serialize(new
            {
                TeamId = team.Id,
                TeamMemberId = member.Id,
                MembershipId = command.MembershipId
            }),
            null,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RemoveTeamMemberResult(
            command.TeamId,
            command.MembershipId);
    }
}
