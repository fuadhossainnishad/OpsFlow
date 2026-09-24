using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.ListTeams;

public sealed class ListTeamsHandler(
    ITenantContext tenantContext,
    ITeamRepository teamRepository)
{
    public async Task<ListTeamsResult> Handle(
        ListTeamsQuery query,
        CancellationToken cancellationToken)
    {
        var teams = await teamRepository.GetOrganizationTeamsAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            cancellationToken);

        return new ListTeamsResult(
            teams.Select(team => new TeamItemResult(
                team.TeamId,
                team.Name,
                team.Description,
                team.TeamLeadMembershipId,
                team.IsArchived,
                team.CreatedAtUtc,
                team.UpdatedAtUtc)).ToList());
    }
}
