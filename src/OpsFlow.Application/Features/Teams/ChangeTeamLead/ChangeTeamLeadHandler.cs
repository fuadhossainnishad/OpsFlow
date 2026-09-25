using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.ChangeTeamLead;

public sealed class ChangeTeamLeadHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IMembershipRepository membershipRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<ChangeTeamLeadResult> Handle(
        ChangeTeamLeadCommand command,
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
            throw new ConflictException(
                "Archived teams cannot have their lead changed.");
        }

        var membership = await membershipRepository.GetByIdAsync(
            organizationId,
            command.MembershipId,
            cancellationToken);

        if (membership is null || !membership.IsActive)
        {
            throw new NotFoundException(
                "Active organization membership was not found.");
        }

        if (!await teamRepository.IsTeamMemberAsync(
                organizationId,
                team.Id,
                membership.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "The team lead must be a member of the team.");
        }

        var beforeJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.TeamLeadMembershipId
        });

        team.SetTeamLead(membership.Id);

        var afterJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.TeamLeadMembershipId
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.lead_changed",
            "team",
            team.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeTeamLeadResult(
            team.Id,
            membership.Id);
    }
}
