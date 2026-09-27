using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class ReportsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanReadDashboardAndProjectReport()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Reports Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLAT");

        var response = await SendAsync(
            HttpMethod.Get,
            "/api/v1/reports/dashboard",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard =
            await response.Content.ReadFromJsonAsync<DashboardSummaryResponse>();

        dashboard.Should().NotBeNull();
        dashboard!.TotalProjects.Should().Be(1);
        dashboard.ActiveProjects.Should().Be(1);
        dashboard.TotalTasks.Should().Be(0);
        dashboard.PendingApprovals.Should().Be(0);
        dashboard.RunningTimers.Should().Be(0);
        dashboard.TrackedSeconds.Should().Be(0);

        var projectResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/reports/projects",
            owner.AccessToken,
            organization.OrganizationId);

        projectResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects =
            await projectResponse.Content
                .ReadFromJsonAsync<List<ProjectReportResponse>>();

        projects.Should().NotBeNull();
        projects.Should().ContainSingle();

        var report = projects![0];
        report.ProjectId.Should().Be(project.Id);
        report.ProjectKey.Should().Be("PLAT");
        report.ProjectName.Should().Be("Platform");
        report.TotalTasks.Should().Be(0);
        report.CompletedTasks.Should().Be(0);
        report.OpenTasks.Should().Be(0);
        report.TrackedSeconds.Should().Be(0);
        report.TimeEntryCount.Should().Be(0);
    }

    [Fact]
    public async Task TimeReportAggregatesCompletedEntriesByProject()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Time Report Organization");

        var firstProject = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            "BACK");

        var secondProject = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Frontend",
            "FRONT");

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        await CreateTimeEntryAsync(
            owner.AccessToken,
            organization.OrganizationId,
            firstProject.Id,
            date,
            90);

        await CreateTimeEntryAsync(
            owner.AccessToken,
            organization.OrganizationId,
            firstProject.Id,
            date,
            30);

        await CreateTimeEntryAsync(
            owner.AccessToken,
            organization.OrganizationId,
            secondProject.Id,
            date,
            45);

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/reports/time?from={date:yyyy-MM-dd}&to={date:yyyy-MM-dd}",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rows =
            await response.Content.ReadFromJsonAsync<List<TimeReportResponse>>();

        rows.Should().NotBeNull();
        rows.Should().HaveCount(2);

        var backend = rows!.Single(x => x.ProjectId == firstProject.Id);
        backend.ProjectName.Should().Be("Backend");
        backend.TrackedSeconds.Should().Be(120 * 60);
        backend.EntryCount.Should().Be(2);

        var frontend = rows.Single(x => x.ProjectId == secondProject.Id);
        frontend.ProjectName.Should().Be("Frontend");
        frontend.TrackedSeconds.Should().Be(45 * 60);
        frontend.EntryCount.Should().Be(1);
    }

    [Fact]
    public async Task ReportsAreIsolatedBetweenOrganizations()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"owner-one-{Guid.NewGuid():N}@example.com");

        var ownerTwo = await RegisterAndLoginAsync(
            $"owner-two-{Guid.NewGuid():N}@example.com");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "First Reports Organization");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Second Reports Organization");

        await CreateProjectAsync(
            ownerOne.AccessToken,
            organizationOne.OrganizationId,
            "First Project",
            "FIRST");

        var secondProject = await CreateProjectAsync(
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            "Second Project",
            "SECOND");

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        await CreateTimeEntryAsync(
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            secondProject.Id,
            date,
            60);

        var dashboardResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/reports/dashboard",
            ownerOne.AccessToken,
            organizationOne.OrganizationId);

        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard =
            await dashboardResponse.Content
                .ReadFromJsonAsync<DashboardSummaryResponse>();

        dashboard.Should().NotBeNull();
        dashboard!.TotalProjects.Should().Be(1);
        dashboard.TrackedSeconds.Should().Be(0);

        var projectResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/reports/projects",
            ownerOne.AccessToken,
            organizationOne.OrganizationId);

        projectResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects =
            await projectResponse.Content
                .ReadFromJsonAsync<List<ProjectReportResponse>>();

        projects.Should().NotBeNull();
        projects.Should().ContainSingle();
        projects![0].ProjectName.Should().Be("First Project");
        projects.Should().NotContain(x => x.ProjectName == "Second Project");

        var timeResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/reports/time?from={date:yyyy-MM-dd}&to={date:yyyy-MM-dd}",
            ownerOne.AccessToken,
            organizationOne.OrganizationId);

        timeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var timeRows =
            await timeResponse.Content
                .ReadFromJsonAsync<List<TimeReportResponse>>();

        timeRows.Should().NotBeNull();
        timeRows.Should().BeEmpty();
    }

    [Fact]
    public async Task ReportsRequireAuthentication()
    {
        var client = factory.CreateClient();

        var dashboardResponse =
            await client.GetAsync("/api/v1/reports/dashboard");

        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var projectsResponse =
            await client.GetAsync("/api/v1/reports/projects");

        projectsResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var timeResponse =
            await client.GetAsync(
                "/api/v1/reports/time?from=2026-09-01&to=2026-09-30");

        timeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TimeReportRejectsInvalidDateRange()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Invalid Report Organization");

        var response = await SendAsync(
            HttpMethod.Get,
            "/api/v1/reports/time?from=2026-09-20&to=2026-09-19",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task CreateTimeEntryAsync(
        string accessToken,
        Guid organizationId,
        Guid projectId,
        DateOnly date,
        int durationMinutes)
    {
        var startedAt = new DateTimeOffset(
            date.ToDateTime(new TimeOnly(9, 0)),
            TimeSpan.Zero);

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            accessToken,
            organizationId,
            new
            {
                projectId,
                taskId = (Guid?)null,
                description = $"Report entry {durationMinutes}",
                startedAtUtc = startedAt,
                endedAtUtc = startedAt.AddMinutes(durationMinutes)
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(string email)
    {
        const string password = "Password123!";
        var client = factory.CreateClient();

        var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Test",
                lastName = "User"
            });

        register.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await login.Content.ReadFromJsonAsync<LoginResponse>();

        result.Should().NotBeNull();

        return new AuthResult(
            result!.AccessToken,
            result.UserId,
            email);
    }

    private async Task<OrganizationResponse> CreateOrganizationAsync(
        string accessToken,
        string name)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            accessToken,
            null,
            new
            {
                name,
                slug =
                    $"{name.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid():N}"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content
                .ReadFromJsonAsync<OrganizationResponse>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<ProjectResponse> CreateProjectAsync(
        string accessToken,
        Guid organizationId,
        string name,
        string key)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/projects",
            accessToken,
            organizationId,
            new
            {
                name,
                key,
                description = "Project integration test"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content
                .ReadFromJsonAsync<ProjectResponse>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string accessToken,
        Guid? organizationId,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                accessToken);

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

        return await factory.CreateClient().SendAsync(request);
    }

    private sealed record AuthResult(
        string AccessToken,
        Guid UserId,
        string Email);

    private sealed record LoginResponse(
        string AccessToken,
        Guid UserId);

    private sealed record OrganizationResponse(
        Guid OrganizationId);

    private sealed record ProjectResponse(
        Guid Id,
        Guid OrganizationId,
        string Name,
        string Key,
        string Description,
        string Status);

    private sealed record DashboardSummaryResponse(
        int TotalProjects,
        int ActiveProjects,
        int TotalTasks,
        int TodoTasks,
        int InProgressTasks,
        int DoneTasks,
        int CancelledTasks,
        int PendingApprovals,
        int RunningTimers,
        long TrackedSeconds);

    private sealed record ProjectReportResponse(
        Guid ProjectId,
        string ProjectKey,
        string ProjectName,
        string Status,
        int TotalTasks,
        int CompletedTasks,
        int OpenTasks,
        long TrackedSeconds,
        int TimeEntryCount);

    private sealed record TimeReportResponse(
        Guid ProjectId,
        string ProjectName,
        long TrackedSeconds,
        int EntryCount);
}
