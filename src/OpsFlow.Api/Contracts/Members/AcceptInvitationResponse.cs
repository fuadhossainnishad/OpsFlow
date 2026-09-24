namespace OpsFlow.Api.Contracts.Members;

public sealed record AcceptInvitationResponse(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId);
