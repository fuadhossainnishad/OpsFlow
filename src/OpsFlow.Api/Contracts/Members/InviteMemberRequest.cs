namespace OpsFlow.Api.Contracts.Members;

public sealed record InviteMemberRequest(
    string Email,
    string RoleName);
