using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.ArchiveTeam;

public sealed class ArchiveTeamHandler(
    ITenantContext tenantContext,
    ITeamRepository teamRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ArchiveTeamResult> Handle(
        ArchiveTeamCommand command,
        CancellationToken cancellationToken)
    {
        var team = await teamRepository.GetByIdAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            throw new NotFoundException("Team was not found.");
        }

        team.Archive();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ArchiveTeamResult(team.Id, team.IsArchived);
    }
}
