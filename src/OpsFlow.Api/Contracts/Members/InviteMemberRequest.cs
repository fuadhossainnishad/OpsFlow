using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Api.Contracts.Members;

public sealed record InviteMemberRequest(
    [EmailAddress]
    string Email,
    string RoleName);
