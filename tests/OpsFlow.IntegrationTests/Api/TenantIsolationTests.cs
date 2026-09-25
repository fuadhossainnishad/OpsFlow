using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class TenantIsolationTests : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TenantIsolationTests(OpsFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UserCannotAccessProjectFromAnotherOrganization()
    {
        var userA = await RegisterAndLoginAsync(
            $"tenant-a-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var userB = await RegisterAndLoginAsync(
            $"tenant-b-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var orgA = await CreateOrganizationAsync(userA.AccessToken, "Tenant A");
        var orgB = await CreateOrganizationAsync(userB.AccessToken, "Tenant B");

        var projectA = await CreateProjectAsync(
            userA.AccessToken,
            orgA.OrganizationId,
            "Project A",
            "PROJA");

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/projects/{projectA.Id}",
            userB.AccessToken,
            orgB.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserCannotUseOrganizationTheyDoNotBelongTo()
    {
        var userA = await RegisterAndLoginAsync(
            $"tenant-a-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var userB = await RegisterAndLoginAsync(
            $"tenant-b-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var orgA = await CreateOrganizationAsync(userA.AccessToken, "Tenant A");

        var response = await SendAsync(
            HttpMethod.Get,
            "/api/v1/projects",
            userB.AccessToken,
            orgA.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UserCanAccessOwnOrganizationProject()
    {
        var user = await RegisterAndLoginAsync(
            $"tenant-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            user.AccessToken,
            "My Organization");

        var project = await CreateProjectAsync(
            user.AccessToken,
            organization.OrganizationId,
            "My Project",
            "MYPROJ");

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/projects/{project.Id}",
            user.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(
        string email,
        string password)
    {
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Test",
                lastName = "User"
            });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password });

        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>();

        login.Should().NotBeNull();

        return new AuthResult(login!.AccessToken);
    }

    private async Task<OrganizationResult> CreateOrganizationAsync(
        string token,
        string name)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            token,
            null,
            new
            {
                name,
                slug = $"{name.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid():N}"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var organization = await response.Content
            .ReadFromJsonAsync<OrganizationResult>();

        organization.Should().NotBeNull();

        return organization!;
    }

    private async Task<ProjectResult> CreateProjectAsync(
        string token,
        Guid organizationId,
        string name,
        string key)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/projects",
            token,
            organizationId,
            new
            {
                name,
                key
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var project = await response.Content
            .ReadFromJsonAsync<ProjectResult>();

        project.Should().NotBeNull();

        return project!;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string token,
        Guid? organizationId,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        if (organizationId.HasValue)
        {
            request.Headers.Add(
                "X-Organization-Id",
                organizationId.Value.ToString());
        }

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await _client.SendAsync(request);
    }

    private sealed record AuthResult(string AccessToken);

    private sealed record LoginResponse(
        string AccessToken,
        string RefreshToken);

    private sealed record OrganizationResult(
        Guid OrganizationId,
        string Name,
        string Slug);

    private sealed record ProjectResult(
        Guid Id,
        string Name,
        string Key);
}
