namespace OpsFlow.Application.Features.Members.InviteMember;

public sealed record InviteMemberResult(
    Guid InvitationId,
    Guid OrganizationId,
    string Email,
    string RoleName,
    DateTimeOffset ExpiresAtUtc,
    string Token);
