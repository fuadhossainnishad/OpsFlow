namespace OpsFlow.Application.Features.Members.InviteMember;

public sealed record InviteMemberCommand(
    string Email,
    string RoleName);
