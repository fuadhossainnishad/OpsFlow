using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Domain.Teams;

namespace OpsFlow.Application.Features.Teams.AddTeamMember;

public sealed class AddTeamMemberHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IMembershipRepository membershipRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<AddTeamMemberResult> Handle(
        AddTeamMemberCommand command,
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

        if (team.IsArchived)
        {
            throw new ConflictException("Archived teams cannot accept members.");
        }

        var membership = await membershipRepository.GetByIdAsync(
            organizationId,
            command.MembershipId,
            cancellationToken);

        if (membership is null || !membership.IsActive)
        {
            throw new NotFoundException("Active organization membership was not found.");
        }

        if (await teamRepository.IsTeamMemberAsync(
                organizationId,
                team.Id,
                membership.Id,
                cancellationToken))
        {
            throw new ConflictException("The member is already part of this team.");
        }

        var teamMember = TeamMember.Create(team.Id, membership.Id);

        await teamRepository.AddMemberAsync(teamMember, cancellationToken);

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.member_added",
            "team",
            team.Id,
            null,
            JsonSerializer.Serialize(new
            {
                TeamId = team.Id,
                TeamMemberId = teamMember.Id,
                MembershipId = membership.Id
            }),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddTeamMemberResult(
            team.Id,
            membership.Id,
            teamMember.Id);
    }
}
