using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests;

public sealed class ApprovalsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateAndApproveTimeEntry()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization,
            "Platform",
            "APP");

        var timeEntry = await CreateStoppedTimeEntryAsync(
            owner.AccessToken,
            organization,
            project);

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization,
            new
            {
                timeEntryId = timeEntry,
                comment = "Please review."
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created =
            await createResponse.Content.ReadFromJsonAsync<ApprovalResponse>();

        created.Should().NotBeNull();
        created!.TimeEntryId.Should().Be(timeEntry);
        created.Status.Should().Be(1);

        var approveResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{created.ApprovalId}/approve",
            owner.AccessToken,
            organization,
            new
            {
                comment = "Approved."
            });

        approveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{created.ApprovalId}",
            owner.AccessToken,
            organization,
            null);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var approved =
            await getResponse.Content.ReadFromJsonAsync<ApprovalEnvelope>();

        approved.Should().NotBeNull();
        approved!.Approval.Status.Should().Be(2);
        approved.Approval.DecisionComment.Should().Be("Approved.");
        approved.Approval.DecidedByUserId.Should().Be(owner.UserId);
        approved.Approval.DecidedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task OwnerCanRejectPendingApproval()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-reject-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Reject Approval Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization,
            "Platform",
            "REJ");

        var timeEntry = await CreateStoppedTimeEntryAsync(
            owner.AccessToken,
            organization,
            project);

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization,
            new
            {
                timeEntryId = timeEntry,
                comment = "Review this entry."
            });

        var created =
            await createResponse.Content.ReadFromJsonAsync<ApprovalResponse>();

        var rejectResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{created!.ApprovalId}/reject",
            owner.AccessToken,
            organization,
            new
            {
                comment = "Rejected for correction."
            });

        rejectResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{created.ApprovalId}",
            owner.AccessToken,
            organization,
            null);

        var rejected =
            await getResponse.Content.ReadFromJsonAsync<ApprovalEnvelope>();

        rejected.Should().NotBeNull();
        rejected!.Approval.Status.Should().Be(3);
        rejected.Approval.DecisionComment.Should().Be("Rejected for correction.");
        rejected.Approval.DecidedByUserId.Should().Be(owner.UserId);
    }

    [Fact]
    public async Task OwnerCanCancelPendingApproval()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-cancel-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Cancel Approval Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization,
            "Platform",
            "CAN");

        var timeEntry = await CreateStoppedTimeEntryAsync(
            owner.AccessToken,
            organization,
            project);

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization,
            new
            {
                timeEntryId = timeEntry,
                comment = "Cancel this request."
            });

        var created =
            await createResponse.Content.ReadFromJsonAsync<ApprovalResponse>();

        var cancelResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{created!.ApprovalId}/cancel",
            owner.AccessToken,
            organization,
            null);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{created.ApprovalId}",
            owner.AccessToken,
            organization,
            null);

        var cancelled =
            await getResponse.Content.ReadFromJsonAsync<ApprovalEnvelope>();

        cancelled.Should().NotBeNull();
        cancelled!.Approval.Status.Should().Be(4);
    }

    private async Task<Guid> CreateStoppedTimeEntryAsync(
        string accessToken,
        Guid organizationId,
        Guid projectId)
    {
        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);
        var endedAt = startedAt.AddHours(1);

        Console.WriteLine(
            $"[APPROVAL DEBUG] Creating time entry: project={projectId}, organization={organizationId}");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            accessToken,
            organizationId,
            new
            {
                projectId,
                taskId = (Guid?)null,
                description = "Approval integration test",
                startedAtUtc = startedAt,
                endedAtUtc = endedAt
            });

        var responseBody = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            $"Time entry creation failed: {responseBody}");

        var entry =
            await response.Content.ReadFromJsonAsync<TimeEntryResponse>();

        entry.Should().NotBeNull();

        return entry!.TimeEntryId;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string accessToken,
        Guid organizationId,
        object? body)
    {
        using var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Headers.Add("X-Organization-Id", organizationId.ToString());

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await factory.CreateClient().SendAsync(request);
    }

    private async Task<(string AccessToken, Guid UserId)> RegisterAndLoginAsync(
        string email,
        string password)
    {
        var client = factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Test",
                lastName = "Owner"
            });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await client.PostAsJsonAsync(
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

        return (login!.AccessToken, login.UserId);
    }

    private async Task<Guid> CreateOrganizationAsync(
        string accessToken,
        string name)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/organizations");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Content = JsonContent.Create(new
        {
            name,
            slug = $"{name.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid():N}"
        });

        var response = await factory.CreateClient().SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var organization =
            await response.Content.ReadFromJsonAsync<OrganizationResponse>();

        organization.Should().NotBeNull();

        return organization!.OrganizationId;
    }

    private async Task<Guid> CreateProjectAsync(
        string accessToken,
        Guid organizationId,
        string name,
        string code)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/projects",
            accessToken,
            organizationId,
            new
            {
                name,
                key = code,
                description = "Approval test project"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"[APPROVAL DEBUG] Create project response: {responseBody}");

        var project =
            await response.Content.ReadFromJsonAsync<ProjectResponse>();

        project.Should().NotBeNull();

        Console.WriteLine(
            $"[APPROVAL DEBUG] Deserialized projectId: {project!.Id}, organization: {organizationId}");

        return project.Id;
    }

    private sealed record LoginResponse(
        string AccessToken,
        Guid UserId);

    private sealed record OrganizationResponse(
        Guid OrganizationId);

    private sealed record ProjectResponse(
        Guid Id);

    private sealed record TimeEntryResponse(
        Guid TimeEntryId);

    private sealed record ApprovalResponse(
        Guid ApprovalId,
        Guid TimeEntryId,
        int Status);

    private sealed record ApprovalEnvelope(
        ApprovalDetails Approval);

    private sealed record ApprovalDetails(
        int Status,
        Guid? DecidedByUserId,
        string? DecisionComment,
        DateTimeOffset? DecidedAtUtc);
}
