using Microsoft.AspNetCore.Authorization;

namespace OpsFlow.Api.Authorization;

public sealed class PermissionRequirement(
    string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
