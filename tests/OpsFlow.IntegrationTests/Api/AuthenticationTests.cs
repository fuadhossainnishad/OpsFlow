using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OpsFlow.Api.Contracts.Identity;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class AuthenticationTests : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(OpsFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task RegisterShouldCreateUser()
    {
        var email = $"integration-{Guid.NewGuid():N}@opsflow.test";

        var request = new RegisterUserRequest(
            email,
            "Integration",
            "User",
            "StrongPassword123!");

        using var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponse>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal(email, result.Email);
        Assert.Equal("Integration", result.FirstName);
        Assert.Equal("User", result.LastName);
    }

    [Fact]
    public async Task LoginShouldReturnAccessAndRefreshTokens()
    {
        var email = $"login-{Guid.NewGuid():N}@opsflow.test";
        const string password = "StrongPassword123!";

        await RegisterAsync(email, password);

        using var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginUserRequest(email, password));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<LoginUserResponse>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
    }

    [Fact]
    public async Task LoginWithInvalidPasswordShouldReturnUnauthorized()
    {
        var email = $"invalid-password-{Guid.NewGuid():N}@opsflow.test";

        await RegisterAsync(email, "StrongPassword123!");

        using var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginUserRequest(email, "WrongPassword123!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MeShouldReturnAuthenticatedUserId()
    {
        var email = $"me-{Guid.NewGuid():N}@opsflow.test";
        const string password = "StrongPassword123!";

        await RegisterAsync(email, password);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginUserRequest(email, password));

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginUserResponse>();

        Assert.NotNull(login);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/auth/me");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(body);
        Assert.Equal(login.UserId, body.UserId);
    }

    private async Task<RegisterUserResponse> RegisterAsync(
        string email,
        string password)
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterUserRequest(
                email,
                "Integration",
                "User",
                password));

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponse>();

        Assert.NotNull(result);

        return result;
    }

    private sealed record CurrentUserResponse(Guid UserId);
}
