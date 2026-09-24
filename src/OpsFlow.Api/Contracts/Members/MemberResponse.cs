namespace OpsFlow.Api.Contracts.Members;

public sealed record MemberResponse(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    Guid RoleId,
    string RoleName,
    bool IsActive,
    DateTimeOffset JoinedAtUtc);
