using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace OpsFlow.IntegrationTests.Api;

public sealed class AuthenticationTests
{
    [Fact]
    public async Task ProtectedProjectsEndpointShouldRequireAuthentication()
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/api/v1/projects");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
