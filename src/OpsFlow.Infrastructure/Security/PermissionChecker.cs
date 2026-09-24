using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Infrastructure.Security;

public sealed class PermissionChecker(
    OpsFlowDbContext dbContext) : IPermissionChecker
{
    public Task<bool> HasPermissionAsync(
        Guid userId,
        Guid organizationId,
        string permissionCode,
        CancellationToken cancellationToken)
    {
        return dbContext.Memberships
            .AsNoTracking()
            .Where(membership =>
                membership.UserId == userId &&
                membership.OrganizationId == organizationId &&
                membership.IsActive)
            .Join(
                dbContext.RolePermissions,
                membership => membership.RoleId,
                rolePermission => rolePermission.RoleId,
                (_, rolePermission) => rolePermission.PermissionId)
            .Join(
                dbContext.Permissions,
                permissionId => permissionId,
                permission => permission.Id,
                (_, permission) => permission.Code)
            .AnyAsync(
                code => code == permissionCode,
                cancellationToken);
    }
}
