namespace OpsFlow.Api.Contracts.Members;

public sealed record MemberStatusResponse(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    bool IsActive);
