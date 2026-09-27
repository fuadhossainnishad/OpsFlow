using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.IntegrationTests.Api;

public sealed class ApprovalsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateGetAndListApproval()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);

        var timeEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddMinutes(90));

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                timeEntryId = timeEntry.Id,
                comment = "Please review this time entry."
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created =
            await createResponse.Content
                .ReadFromJsonAsync<CreateApprovalResponse>();

        created.Should().NotBeNull();
        created!.OrganizationId.Should().Be(organization.OrganizationId);
        created.RequesterUserId.Should().Be(owner.UserId);
        created.TimeEntryId.Should().Be(timeEntry.Id);
        created.Comment.Should().Be("Please review this time entry.");
        created.Status.Should().Be(ApprovalStatus.Pending);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{created.ApprovalId}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult =
            await getResponse.Content
                .ReadFromJsonAsync<GetApprovalResponse>();

        getResult.Should().NotBeNull();
        getResult!.Approval.Id.Should().Be(created.ApprovalId);
        getResult.Approval.Status.Should().Be(ApprovalStatus.Pending);

        var listResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/approvals",
            owner.AccessToken,
            organization.OrganizationId);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResult =
            await listResponse.Content
                .ReadFromJsonAsync<ListApprovalsResponse>();

        listResult.Should().NotBeNull();
        listResult!.Approvals.Should().ContainSingle();
        listResult.Approvals[0].Id.Should().Be(created.ApprovalId);
    }

    [Fact]
    public async Task ApprovalCanBeApprovedAndCannotBeChangedAgain()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-approve-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Approve Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddHours(-3);

        var timeEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddHours(1));

        var approval = await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            timeEntry.Id);

        var approveResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{approval.ApprovalId}/approve",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                comment = "Approved."
            });

        approveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{approval.ApprovalId}",
            owner.AccessToken,
            organization.OrganizationId);

        var result =
            await getResponse.Content
                .ReadFromJsonAsync<GetApprovalResponse>();

        result.Should().NotBeNull();
        result!.Approval.Status.Should().Be(ApprovalStatus.Approved);
        result.Approval.DecisionComment.Should().Be("Approved.");
        result.Approval.DecidedByUserId.Should().Be(owner.UserId);
        result.Approval.DecidedAtUtc.Should().NotBeNull();

        var rejectResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{approval.ApprovalId}/reject",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                comment = "Too late."
            });

        rejectResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ApprovalCanBeRejected()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-reject-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Reject Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddHours(-4);

        var timeEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddMinutes(45));

        var approval = await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            timeEntry.Id);

        var response = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{approval.ApprovalId}/reject",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                comment = "Please correct the entry."
            });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{approval.ApprovalId}",
            owner.AccessToken,
            organization.OrganizationId);

        var result =
            await getResponse.Content
                .ReadFromJsonAsync<GetApprovalResponse>();

        result.Should().NotBeNull();
        result!.Approval.Status.Should().Be(ApprovalStatus.Rejected);
        result.Approval.DecisionComment.Should().Be(
            "Please correct the entry.");
    }

    [Fact]
    public async Task RequesterCanCancelPendingApproval()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-cancel-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Cancel Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddHours(-5);

        var timeEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddMinutes(30));

        var approval = await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            timeEntry.Id);

        var response = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{approval.ApprovalId}/cancel",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{approval.ApprovalId}",
            owner.AccessToken,
            organization.OrganizationId);

        var result =
            await getResponse.Content
                .ReadFromJsonAsync<GetApprovalResponse>();

        result.Should().NotBeNull();
        result!.Approval.Status.Should().Be(ApprovalStatus.Cancelled);
        result.Approval.DecidedByUserId.Should().Be(owner.UserId);
    }

    [Fact]
    public async Task CannotCreateSecondPendingApprovalForSameTimeEntry()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-duplicate-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Duplicate Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddHours(-6);

        var timeEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddMinutes(20));

        var first = await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            timeEntry.Id);

        first.Status.Should().Be(ApprovalStatus.Pending);

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                timeEntryId = timeEntry.Id,
                comment = "Duplicate approval."
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RunningTimeEntryCannotBeSubmittedForApproval()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-running-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Running Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Running work",
                startedAtUtc = (DateTimeOffset?)null,
                endedAtUtc = (DateTimeOffset?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var timer =
            await response.Content
                .ReadFromJsonAsync<TimeEntryResponse>();

        timer.Should().NotBeNull();

        var approvalResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                timeEntryId = timer!.Id,
                comment = "Should fail."
            });

        approvalResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ApprovalsAreIsolatedBetweenOrganizations()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"approval-tenant-one-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var ownerTwo = await RegisterAndLoginAsync(
            $"approval-tenant-two-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "Approval Tenant One");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Approval Tenant Two");

        var project = await CreateProjectAsync(
            ownerOne.AccessToken,
            organizationOne.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var startedAt = DateTimeOffset.UtcNow.AddDays(-1);

        var timeEntry = await CreateManualTimeEntryAsync(
            ownerOne,
            organizationOne.OrganizationId,
            project.ProjectId,
            startedAt,
            startedAt.AddMinutes(60));

        var approval = await CreateApprovalAsync(
            ownerOne,
            organizationOne.OrganizationId,
            timeEntry.Id);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals/{approval.ApprovalId}",
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var listResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/approvals",
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list =
            await listResponse.Content
                .ReadFromJsonAsync<ListApprovalsResponse>();

        list.Should().NotBeNull();
        list!.Approvals.Should().BeEmpty();
    }

    [Fact]
    public async Task ApprovalsRequireAuthentication()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/approvals");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ApprovalsCanBeFilteredByStatusAndRequester()
    {
        var owner = await RegisterAndLoginAsync(
            $"approval-filter-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Approval Filter Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            $"APR-{Guid.NewGuid():N}"[..8]);

        var firstStartedAt = DateTimeOffset.UtcNow.AddHours(-8);
        var secondStartedAt = DateTimeOffset.UtcNow.AddHours(-7);

        var firstEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            firstStartedAt,
            firstStartedAt.AddMinutes(30));

        var secondEntry = await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            project.ProjectId,
            secondStartedAt,
            secondStartedAt.AddMinutes(30));

        var firstApproval = await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            firstEntry.Id);

        await CreateApprovalAsync(
            owner,
            organization.OrganizationId,
            secondEntry.Id);

        var approveResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/approvals/{firstApproval.ApprovalId}/approve",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                comment = "Approved."
            });

        approveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var statusResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/approvals?status=Approved",
            owner.AccessToken,
            organization.OrganizationId);

        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var statusResult =
            await statusResponse.Content
                .ReadFromJsonAsync<ListApprovalsResponse>();

        statusResult.Should().NotBeNull();
        statusResult!.Approvals.Should().ContainSingle();
        statusResult.Approvals[0].Id.Should().Be(firstApproval.ApprovalId);

        var requesterResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/approvals?requesterUserId={owner.UserId}",
            owner.AccessToken,
            organization.OrganizationId);

        requesterResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var requesterResult =
            await requesterResponse.Content
                .ReadFromJsonAsync<ListApprovalsResponse>();

        requesterResult.Should().NotBeNull();
        requesterResult!.Approvals.Should().HaveCount(2);
        requesterResult.Approvals.Should().OnlyContain(
            x => x.RequesterUserId == owner.UserId);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(
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
                lastName = "User"
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

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        loginResult.Should().NotBeNull();

        return new AuthResult(
            loginResult!.AccessToken,
            loginResult.UserId,
            email);
    }

    private async Task<OrganizationResult> CreateOrganizationAsync(
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
                .ReadFromJsonAsync<OrganizationResult>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<ProjectResult> CreateProjectAsync(
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
                description = "Approval integration test"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content
                .ReadFromJsonAsync<ProjectResult>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<TimeEntryResponse> CreateManualTimeEntryAsync(
        AuthResult user,
        Guid organizationId,
        Guid projectId,
        DateTimeOffset startedAt,
        DateTimeOffset endedAt)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            user.AccessToken,
            organizationId,
            new
            {
                projectId,
                taskId = (Guid?)null,
                description = "Approval test work",
                startedAtUtc = startedAt,
                endedAtUtc = endedAt
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<TimeEntryResponse>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<CreateApprovalResponse> CreateApprovalAsync(
        AuthResult user,
        Guid organizationId,
        Guid timeEntryId)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/approvals",
            user.AccessToken,
            organizationId,
            new
            {
                timeEntryId,
                comment = "Approval integration test"
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<CreateApprovalResponse>();

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

    private sealed record OrganizationResult(
        Guid OrganizationId);

    private sealed record ProjectResult(
        Guid Id,
        Guid OrganizationId,
        string Name,
        string Key,
        string Description,
        string Status)
    {
        public Guid ProjectId => Id;
    }

    private sealed record TimeEntryResponse(
        Guid TimeEntryId)
    {
        public Guid Id => TimeEntryId;
    }

    private sealed record CreateApprovalResponse(
        Guid ApprovalId,
        Guid OrganizationId,
        Guid RequesterUserId,
        Guid TimeEntryId,
        string? Comment,
        ApprovalStatus Status,
        DateTimeOffset CreatedAtUtc);

    private sealed record ApprovalResponse(
        Guid Id,
        Guid OrganizationId,
        Guid RequesterUserId,
        Guid TimeEntryId,
        string? Comment,
        ApprovalStatus Status,
        Guid? DecidedByUserId,
        string? DecisionComment,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? DecidedAtUtc);

    private sealed record GetApprovalResponse(
        ApprovalResponse Approval);

    private sealed record ListApprovalsResponse(
        IReadOnlyList<ApprovalResponse> Approvals);
}
