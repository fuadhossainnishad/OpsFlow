using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.IntegrationTests.Infrastructure;

public sealed class OpsFlowWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ConnectionString =
        "Server=localhost,1433;Database=OpsFlowIntegrationDb;User Id=sa;Password=OpsFlow_Local_Sql;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");


        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:OpsFlowDatabase"] = ConnectionString,
                ["Jwt:Issuer"] = "OpsFlow.IntegrationTests",
                ["Jwt:Audience"] = "OpsFlow.IntegrationTests",
                ["Jwt:SigningKey"] = "OpsFlowIntegrationTestSigningKey-AtLeast32CharactersLong",
                ["Jwt:AccessTokenLifetimeMinutes"] = "30"
            };

            configuration.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(descriptor =>
                    descriptor.ServiceType == typeof(DbContextOptions<OpsFlowDbContext>) ||
                    descriptor.ServiceType == typeof(OpsFlowDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddDbContext<OpsFlowDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            using var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<OpsFlowDbContext>();

            dbContext.Database.Migrate();
        });
    }
}
