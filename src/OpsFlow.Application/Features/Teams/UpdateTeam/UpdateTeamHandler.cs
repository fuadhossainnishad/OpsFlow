using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;

namespace OpsFlow.Application.Features.Teams.UpdateTeam;

public sealed class UpdateTeamHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateTeamResult> Handle(
        UpdateTeamCommand command,
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

        var normalizedName = command.Name.Trim().ToUpperInvariant();

        if (await teamRepository.ExistsByNormalizedNameAsync(
                organizationId,
                normalizedName,
                team.Id,
                cancellationToken))
        {
            throw new ConflictException("A team with this name already exists.");
        }

        var beforeJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.Name,
            team.Description,
            team.IsArchived
        });

        team.Update(command.Name, command.Description);

        var afterJson = JsonSerializer.Serialize(new
        {
            team.Id,
            team.Name,
            team.Description,
            team.IsArchived
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.updated",
            "team",
            team.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTeamResult(
            team.Id,
            team.Name,
            team.Description,
            team.UpdatedAtUtc);
    }
}
