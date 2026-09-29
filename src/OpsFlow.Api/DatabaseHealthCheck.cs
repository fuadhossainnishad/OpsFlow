using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Api;

public sealed class DatabaseHealthCheck(OpsFlowDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database
            .CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Database is reachable.")
            : HealthCheckResult.Unhealthy("Database is unavailable.");
    }
}
