using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Domain.Teams;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class TeamRepository(
    OpsFlowDbContext dbContext) : ITeamRepository
{
    public async Task AddAsync(
        Team team,
        CancellationToken cancellationToken)
    {
        await dbContext.Teams.AddAsync(team, cancellationToken);
    }

    public Task<Team?> GetByIdAsync(
        Guid organizationId,
        Guid teamId,
        CancellationToken cancellationToken)
    {
        return dbContext.Teams
            .SingleOrDefaultAsync(
                team => team.OrganizationId == organizationId &&
                        team.Id == teamId,
                cancellationToken);
    }

    public Task<bool> ExistsByNormalizedNameAsync(
        Guid organizationId,
        string normalizedName,
        Guid? excludingTeamId,
        CancellationToken cancellationToken)
    {
        return dbContext.Teams.AnyAsync(
            team => team.OrganizationId == organizationId &&
                    team.NormalizedName == normalizedName &&
                    (!excludingTeamId.HasValue || team.Id != excludingTeamId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<TeamRecord>> GetOrganizationTeamsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .AsNoTracking()
            .Where(team => team.OrganizationId == organizationId)
            .OrderBy(team => team.Name)
            .Select(team => new TeamRecord(
                team.Id,
                team.OrganizationId,
                team.Name,
                team.Description,
                team.TeamLeadMembershipId,
                team.IsArchived,
                team.CreatedAtUtc,
                team.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TeamMemberRecord>> GetTeamMembersAsync(
        Guid organizationId,
        Guid teamId,
        CancellationToken cancellationToken)
    {
        return await (
            from teamMember in dbContext.TeamMembers.AsNoTracking()
            join membership in dbContext.Memberships.AsNoTracking()
                on teamMember.MembershipId equals membership.Id
            join user in dbContext.Users.AsNoTracking()
                on membership.UserId equals user.Id
            where teamMember.TeamId == teamId &&
                  membership.OrganizationId == organizationId
            orderby user.FirstName, user.LastName
            select new TeamMemberRecord(
                teamMember.Id,
                teamMember.MembershipId,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                membership.IsActive,
                teamMember.AddedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsTeamMemberAsync(
        Guid organizationId,
        Guid teamId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        return (
            from teamMember in dbContext.TeamMembers
            join membership in dbContext.Memberships
                on teamMember.MembershipId equals membership.Id
            where teamMember.TeamId == teamId &&
                  teamMember.MembershipId == membershipId &&
                  membership.OrganizationId == organizationId
            select teamMember.Id)
            .AnyAsync(cancellationToken);
    }

    public async Task AddMemberAsync(
        TeamMember teamMember,
        CancellationToken cancellationToken)
    {
        await dbContext.TeamMembers.AddAsync(teamMember, cancellationToken);
    }

    public Task<TeamMember?> GetTeamMemberAsync(
        Guid organizationId,
        Guid teamId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        return (
            from teamMember in dbContext.TeamMembers
            join membership in dbContext.Memberships
                on teamMember.MembershipId equals membership.Id
            where teamMember.TeamId == teamId &&
                  teamMember.MembershipId == membershipId &&
                  membership.OrganizationId == organizationId
            select teamMember)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void RemoveMember(TeamMember teamMember)
    {
        dbContext.TeamMembers.Remove(teamMember);
    }
}
