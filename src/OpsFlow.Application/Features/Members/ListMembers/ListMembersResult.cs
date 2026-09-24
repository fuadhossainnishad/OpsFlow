namespace OpsFlow.Application.Features.Members.ListMembers;

public sealed record ListMembersResult(
    IReadOnlyList<MemberResult> Members);

public sealed record MemberResult(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    Guid RoleId,
    string RoleName,
    bool IsActive,
    DateTimeOffset JoinedAtUtc);
