using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Api.Identity;

public sealed class TenantContext(
    ICurrentUser currentUser,
    OpsFlowDbContext dbContext) : ITenantContext
{
    private Guid? _organizationId;

    public Guid OrganizationId
    {
        get
        {
            if (_organizationId.HasValue)
            {
                return _organizationId.Value;
            }

            var organizationId = dbContext.Memberships
                .AsNoTracking()
                .Where(membership =>
                    membership.UserId == currentUser.UserId &&
                    membership.IsActive)
                .Select(membership => membership.OrganizationId)
                .FirstOrDefault();

            if (organizationId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "Authenticated user does not belong to an organization.");
            }

            _organizationId = organizationId;

            return organizationId;
        }
    }
}
