namespace OpsFlow.Application.Abstractions.Authorization;

public interface IPermissionChecker
{
    Task<bool> HasPermissionAsync(
        Guid userId,
        Guid organizationId,
        string permissionCode,
        CancellationToken cancellationToken);
}
