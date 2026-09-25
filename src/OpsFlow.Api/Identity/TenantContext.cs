using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Api.Identity;

public sealed class TenantContext(
    ICurrentUser currentUser,
    OpsFlowDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private Guid? _organizationId;

    public async Task<Guid> GetOrganizationIdAsync(
        CancellationToken cancellationToken)
    {
        if (_organizationId.HasValue)
        {
            return _organizationId.Value;
        }

        var headerValue = httpContextAccessor
            .HttpContext?
            .Request
            .Headers["X-Organization-Id"]
            .FirstOrDefault();

        if (!Guid.TryParse(headerValue, out var organizationId))
        {
            throw new BadHttpRequestException(
                "A valid X-Organization-Id header is required.");
        }

        var hasMembership = await dbContext.Memberships
            .AsNoTracking()
            .AnyAsync(
                membership =>
                    membership.UserId == currentUser.UserId &&
                    membership.OrganizationId == organizationId &&
                    membership.IsActive,
                cancellationToken);

        if (!hasMembership)
        {
            throw new ForbiddenException(
                "Authenticated user does not belong to the selected organization.");
        }

        _organizationId = organizationId;

        return organizationId;
    }
}
