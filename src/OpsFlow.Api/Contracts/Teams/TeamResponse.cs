namespace OpsFlow.Api.Contracts.Teams;

public sealed record TeamResponse(
    Guid TeamId,
    Guid OrganizationId,
    string Name,
    string? Description,
    Guid? TeamLeadMembershipId,
    bool IsArchived,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    IReadOnlyList<TeamMemberResponse>? Members = null);

public sealed record TeamMemberResponse(
    Guid TeamMemberId,
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTimeOffset AddedAtUtc);
