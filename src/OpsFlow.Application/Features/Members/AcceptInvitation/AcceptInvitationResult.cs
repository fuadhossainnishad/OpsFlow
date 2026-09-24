namespace OpsFlow.Application.Features.Members.AcceptInvitation;

public sealed record AcceptInvitationResult(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId);
