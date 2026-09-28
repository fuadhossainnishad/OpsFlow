using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;
using OpsFlow.Api.Contracts.Identity;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class OpenApiContractTests : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OpenApiContractTests(OpsFlowWebApplicationFactory factory)
    {
        factory.EnableOpenApi();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OpenApiDocumentDescribesBearerSecurityAndOperationErrors()
    {
        using var response = await _client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = document.RootElement;
        var bearer = root
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");

        Assert.Equal("http", bearer.GetProperty("type").GetString());
        Assert.Equal("bearer", bearer.GetProperty("scheme").GetString());
        Assert.Equal("JWT", bearer.GetProperty("bearerFormat").GetString());

        var paths = root.GetProperty("paths");
        var register = paths
            .GetProperty("/api/v1/auth/register")
            .GetProperty("post");
        var login = paths
            .GetProperty("/api/v1/auth/login")
            .GetProperty("post");
        var protectedMe = paths
            .GetProperty("/api/v1/auth/me")
            .GetProperty("get");
        var createTeam = paths
            .GetProperty("/api/v1/teams")
            .GetProperty("post");

        Assert.False(register.TryGetProperty("security", out _));
        Assert.False(login.TryGetProperty("security", out _));
        AssertBearerSecurity(protectedMe);
        AssertBearerSecurity(createTeam);

        Assert.True(register.GetProperty("responses").TryGetProperty("400", out _));
        Assert.True(createTeam.GetProperty("responses").TryGetProperty("409", out _));
        Assert.True(protectedMe.GetProperty("responses").TryGetProperty("401", out _));
    }

    [Fact]
    public async Task UnmatchedRouteReturnsProblemDetailsWithTraceId()
    {
        var email = $"openapi-{Guid.NewGuid():N}@opsflow.test";
        const string password = "StrongPassword123!";
        using var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterUserRequest(email, "OpenAPI", "Test", password));
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        using var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginUserRequest(email, password));
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();
        Assert.NotNull(login);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/does-not-exist");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);
        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, body.RootElement.GetProperty("status").GetInt32());
        Assert.True(body.RootElement.TryGetProperty("traceId", out _));
    }

    private static void AssertBearerSecurity(JsonElement operation)
    {
        Assert.Equal(
            "Bearer",
            operation.GetProperty("security")[0]
                .EnumerateObject()
                .Single()
                .Name);
    }
}
