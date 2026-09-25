using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Domain.Teams;

namespace OpsFlow.Application.Features.Teams.CreateTeam;

public sealed class CreateTeamHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITeamRepository teamRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateTeamResult> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var normalizedName = command.Name.Trim().ToUpperInvariant();

        if (await teamRepository.ExistsByNormalizedNameAsync(
                organizationId,
                normalizedName,
                null,
                cancellationToken))
        {
            throw new ConflictException("A team with this name already exists.");
        }

        var team = Team.Create(
            organizationId,
            command.Name,
            command.Description);

        await teamRepository.AddAsync(team, cancellationToken);

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "team.created",
            "team",
            team.Id,
            null,
            JsonSerializer.Serialize(new
            {
                team.Id,
                team.Name,
                team.Description,
                team.IsArchived
            }),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTeamResult(
            team.Id,
            team.OrganizationId,
            team.Name,
            team.Description,
            team.IsArchived,
            team.CreatedAtUtc);
    }
}
