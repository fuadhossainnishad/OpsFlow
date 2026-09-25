using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;
using OpsFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace OpsFlow.IntegrationTests.Messaging;

public sealed class OutboxTests : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpsFlowWebApplicationFactory _factory;

    public OutboxTests(OpsFlowWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatingTaskShouldCreateOutboxMessage()
    {
        var user = await RegisterAndLoginAsync(
            $"outbox-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            user.AccessToken,
            "Outbox Organization");

        var project = await CreateProjectAsync(
            user.AccessToken,
            organization.OrganizationId,
            "Outbox Project",
            $"OUT{Random.Shared.Next(100, 999)}");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/tasks",
            user.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.Id,
                title = "Outbox integration test"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var scope = _factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<OpsFlowDbContext>();

        var outboxMessage = await dbContext.OutboxMessages
            .OrderByDescending(message => message.OccurredAt)
            .FirstAsync();

        outboxMessage.MessageType
            .Should()
            .Be("task.created");

        outboxMessage.Payload
            .Should()
            .Contain(project.Id.ToString());

        outboxMessage.ProcessedAt
            .Should()
            .BeNull();
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
                firstName = "Outbox",
                lastName = "Test"
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

        var organization =
            await response.Content.ReadFromJsonAsync<OrganizationResult>();

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

        var project =
            await response.Content.ReadFromJsonAsync<ProjectResult>();

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
