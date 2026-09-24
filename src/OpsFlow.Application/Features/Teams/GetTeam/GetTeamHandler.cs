using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.GetTeam;

public sealed class GetTeamHandler(
    ITenantContext tenantContext,
    ITeamRepository teamRepository)
{
    public async Task<GetTeamResult> Handle(
        GetTeamQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var team = await teamRepository.GetByIdAsync(
            organizationId,
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            throw new NotFoundException("Team was not found.");
        }

        var members = await teamRepository.GetTeamMembersAsync(
            organizationId,
            team.Id,
            cancellationToken);

        return new GetTeamResult(
            team.Id,
            team.OrganizationId,
            team.Name,
            team.Description,
            team.TeamLeadMembershipId,
            team.IsArchived,
            team.CreatedAtUtc,
            team.UpdatedAtUtc,
            members.Select(member => new TeamMemberResult(
                member.TeamMemberId,
                member.MembershipId,
                member.UserId,
                member.Email,
                member.FirstName,
                member.LastName,
                member.IsActive,
                member.AddedAtUtc)).ToList());
    }
}
