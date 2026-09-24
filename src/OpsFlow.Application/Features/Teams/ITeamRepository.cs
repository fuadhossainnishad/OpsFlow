using OpsFlow.Domain.Teams;

namespace OpsFlow.Application.Features.Teams;

public interface ITeamRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken);

    Task<Team?> GetByIdAsync(
        Guid organizationId,
        Guid teamId,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNormalizedNameAsync(
        Guid organizationId,
        string normalizedName,
        Guid? excludingTeamId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamRecord>> GetOrganizationTeamsAsync(
        Guid organizationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamMemberRecord>> GetTeamMembersAsync(
        Guid organizationId,
        Guid teamId,
        CancellationToken cancellationToken);

    Task<bool> IsTeamMemberAsync(
        Guid organizationId,
        Guid teamId,
        Guid membershipId,
        CancellationToken cancellationToken);

    Task AddMemberAsync(
        TeamMember teamMember,
        CancellationToken cancellationToken);

    Task<TeamMember?> GetTeamMemberAsync(
        Guid organizationId,
        Guid teamId,
        Guid membershipId,
        CancellationToken cancellationToken);

    void RemoveMember(TeamMember teamMember);
}

public sealed record TeamRecord(
    Guid TeamId,
    Guid OrganizationId,
    string Name,
    string? Description,
    Guid? TeamLeadMembershipId,
    bool IsArchived,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record TeamMemberRecord(
    Guid TeamMemberId,
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTimeOffset AddedAtUtc);
