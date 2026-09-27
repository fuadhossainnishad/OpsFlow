using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class AuditLogsTests : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuditLogsTests(OpsFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OwnerCanListAuditLogsWithPaginationAndFilters()
    {
        var owner = await RegisterAndLoginAsync(
            $"audit-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Audit Test Organization");

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/audit-logs?page=1&pageSize=1");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", owner.AccessToken);
        request.Headers.Add("X-Organization-Id", organization.OrganizationId.ToString());

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<ListAuditLogsResponse>();

        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.Items.Should().NotBeEmpty();

        result.Items.Should().Contain(x =>
            x.Resource == "organization" &&
            x.ResourceId == organization.OrganizationId);

        var filteredRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/audit-logs?resource=organization&resourceId={organization.OrganizationId}");

        filteredRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", owner.AccessToken);
        filteredRequest.Headers.Add("X-Organization-Id", organization.OrganizationId.ToString());

        var filteredResponse = await _client.SendAsync(filteredRequest);

        filteredResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var filtered =
            await filteredResponse.Content
                .ReadFromJsonAsync<ListAuditLogsResponse>();

        filtered.Should().NotBeNull();
        filtered!.Items.Should().NotBeEmpty();
        filtered.Items.Should().OnlyContain(x =>
            x.Resource == "organization" &&
            x.ResourceId == organization.OrganizationId);
    }

    [Fact]
    public async Task AuditLogsAreIsolatedBetweenOrganizations()
    {
        var ownerA = await RegisterAndLoginAsync(
            $"audit-a-{Guid.NewGuid():N}@example.com");

        var ownerB = await RegisterAndLoginAsync(
            $"audit-b-{Guid.NewGuid():N}@example.com");

        var organizationA = await CreateOrganizationAsync(
            ownerA.AccessToken,
            "Audit Organization A");

        var organizationB = await CreateOrganizationAsync(
            ownerB.AccessToken,
            "Audit Organization B");

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/audit-logs");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerB.AccessToken);
        request.Headers.Add("X-Organization-Id", organizationB.OrganizationId.ToString());

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<ListAuditLogsResponse>();

        result.Should().NotBeNull();
        result!.Items.Should().NotContain(x =>
            x.ResourceId == organizationA.OrganizationId);
    }

    [Fact]
    public async Task AuditLogsRequireAuthentication()
    {
        var response = await _client.GetAsync("/api/v1/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(string email)
    {
        var password = "TestPassword123!";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Audit",
                lastName = "Tester"
            });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        login.Should().NotBeNull();
        return new AuthResult(login!.AccessToken);
    }

    private async Task<OrganizationResult> CreateOrganizationAsync(
        string accessToken,
        string name)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/organizations");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = JsonContent.Create(new
        {
            name,
            slug = $"{name.ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}"[..50]
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var organization =
            await response.Content.ReadFromJsonAsync<OrganizationResult>();

        organization.Should().NotBeNull();
        return organization!;
    }

    private sealed record AuthResult(string AccessToken);

    private sealed record LoginResponse(string AccessToken);

    private sealed record OrganizationResult(
        Guid OrganizationId);

    private sealed record ListAuditLogsResponse(
        IReadOnlyList<AuditLogItemResponse> Items,
        int Page,
        int PageSize,
        bool HasNextPage);

    private sealed record AuditLogItemResponse(
        Guid Id,
        Guid? ActorUserId,
        string Action,
        string Resource,
        Guid? ResourceId,
        DateTimeOffset OccurredAtUtc,
        string? IpAddress,
        string? UserAgent,
        string? CorrelationId,
        string? BeforeJson,
        string? AfterJson);
}
