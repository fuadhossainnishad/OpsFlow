using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.ArchiveTeam;

public sealed class ArchiveTeamHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<ArchiveTeamResult> Handle(
        ArchiveTeamCommand command,
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

        var beforeJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.Name,
            team.IsArchived
        });

        team.Archive();

        var afterJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.Name,
            team.IsArchived
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.archived",
            "team",
            team.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ArchiveTeamResult(team.Id, team.IsArchived);
    }
}
