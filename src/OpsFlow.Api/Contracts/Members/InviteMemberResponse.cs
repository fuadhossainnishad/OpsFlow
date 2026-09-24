namespace OpsFlow.Api.Contracts.Members;

public sealed record InviteMemberResponse(
    Guid InvitationId,
    Guid OrganizationId,
    string Email,
    string RoleName,
    DateTimeOffset ExpiresAtUtc,
    string Token);
