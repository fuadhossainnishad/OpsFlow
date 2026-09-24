namespace OpsFlow.Application.Features.Teams.GetTeam;

public sealed record GetTeamResult(
    Guid TeamId,
    Guid OrganizationId,
    string Name,
    string? Description,
    Guid? TeamLeadMembershipId,
    bool IsArchived,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    IReadOnlyList<TeamMemberResult> Members);

public sealed record TeamMemberResult(
    Guid TeamMemberId,
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTimeOffset AddedAtUtc);
