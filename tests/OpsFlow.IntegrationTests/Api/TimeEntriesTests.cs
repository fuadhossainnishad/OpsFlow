using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.IntegrationTests.Api;

public sealed class TimeEntriesTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateManualTimeEntryAndRetrieveIt()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Time Entry Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLAT");

        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);
        var endedAt = startedAt.AddMinutes(90);

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Backend implementation",
                startedAtUtc = startedAt,
                endedAtUtc = endedAt
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created =
            await createResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        created.Should().NotBeNull();
        created!.ProjectId.Should().Be(project.ProjectId);
        created.Description.Should().Be("Backend implementation");
        created.IsManual.Should().BeTrue();
        created.EndedAtUtc.Should().NotBeNull();
        created.DurationSeconds.Should().Be(90 * 60);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/time-entries/{created.TimeEntryId}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrieved =
            await getResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        retrieved.Should().NotBeNull();
        retrieved!.TimeEntryId.Should().Be(created.TimeEntryId);
        retrieved.DurationSeconds.Should().Be(90 * 60);
    }

    [Fact]
    public async Task OwnerCanStartRunningTimerAndSecondRunningTimerIsRejected()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Running Timer Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "RUN");

        var firstResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "First running timer",
                startedAtUtc = (DateTimeOffset?)null,
                endedAtUtc = (DateTimeOffset?)null
            });

        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var first =
            await firstResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        first.Should().NotBeNull();
        first!.EndedAtUtc.Should().BeNull();
        first.DurationSeconds.Should().BeNull();
        first.IsManual.Should().BeFalse();

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Second running timer",
                startedAtUtc = (DateTimeOffset?)null,
                endedAtUtc = (DateTimeOffset?)null
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UserCanUpdateAndDeleteTimeEntry()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Time Entry CRUD Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "CRUD");

        var startedAt = DateTimeOffset.UtcNow.AddHours(-3);
        var endedAt = startedAt.AddHours(1);

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Initial description",
                startedAtUtc = startedAt,
                endedAtUtc = endedAt
            });

        var created =
            await createResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        var updatedStartedAt = startedAt.AddMinutes(15);
        var updatedEndedAt = updatedStartedAt.AddMinutes(45);

        var updateResponse = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/time-entries/{created!.TimeEntryId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                taskId = (Guid?)null,
                description = "Updated description",
                startedAtUtc = updatedStartedAt,
                endedAtUtc = updatedEndedAt
            });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated =
            await updateResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        updated.Should().NotBeNull();
        updated!.Description.Should().Be("Updated description");
        updated.DurationSeconds.Should().Be(45 * 60);

        var deleteResponse = await SendAsync(
            HttpMethod.Delete,
            $"/api/v1/time-entries/{created.TimeEntryId}",
            owner.AccessToken,
            organization.OrganizationId);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/time-entries/{created.TimeEntryId}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserCannotCreateTimeEntryForProjectFromAnotherOrganization()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"owner-one-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var ownerTwo = await RegisterAndLoginAsync(
            $"owner-two-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "First Time Organization");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Second Time Organization");

        var project = await CreateProjectAsync(
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            "Private Project",
            "PRIVATE");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            ownerOne.AccessToken,
            organizationOne.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Cross tenant attempt",
                startedAtUtc = DateTimeOffset.UtcNow.AddHours(-1),
                endedAtUtc = DateTimeOffset.UtcNow
            });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TimeEntryCannotUseTaskFromAnotherProject()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Task Validation Organization");

        var projectOne = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            "BACK");

        var projectTwo = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Frontend",
            "FRONT");

        var task = await CreateTaskAsync(
            owner.AccessToken,
            organization.OrganizationId,
            projectOne.ProjectId,
            "Backend task");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = projectTwo.ProjectId,
                taskId = task.TaskId,
                description = "Invalid task/project combination",
                startedAtUtc = DateTimeOffset.UtcNow.AddHours(-1),
                endedAtUtc = DateTimeOffset.UtcNow
            });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UserCannotAccessAnotherUsersTimeEntry()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Time Ownership Organization");

        await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Shared Project",
            "SHARED");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            member.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Member work",
                startedAtUtc = DateTimeOffset.UtcNow.AddHours(-1),
                endedAtUtc = DateTimeOffset.UtcNow
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created =
            await createResponse.Content.ReadFromJsonAsync<TimeEntryResponse>();

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/time-entries/{created!.TimeEntryId}",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegularMemberCanCreateAndListOwnTimeEntries()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Member Time Organization");

        await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Member Project",
            "MEM");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/time-entries",
            member.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.ProjectId,
                taskId = (Guid?)null,
                description = "Member work",
                startedAtUtc = DateTimeOffset.UtcNow.AddHours(-2),
                endedAtUtc = DateTimeOffset.UtcNow.AddHours(-1)
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/time-entries",
            member.AccessToken,
            organization.OrganizationId);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list =
            await listResponse.Content.ReadFromJsonAsync<TimeEntryListResponse>();

        list.Should().NotBeNull();
        list!.Items.Should().ContainSingle();
        list.Items[0].Description.Should().Be("Member work");
    }

    [Fact]
    public async Task TimeEntryListCanFilterByProjectAndDateRange()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Time Filter Organization");

        var projectOne = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Backend",
            "FILTER-B");

        var projectTwo = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Frontend",
            "FILTER-F");

        var baseTime = DateTimeOffset.UtcNow.AddDays(-2);

        await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            projectOne.ProjectId,
            "Backend work",
            baseTime,
            baseTime.AddHours(1));

        await CreateManualTimeEntryAsync(
            owner,
            organization.OrganizationId,
            projectTwo.ProjectId,
            "Frontend work",
            baseTime.AddDays(1),
            baseTime.AddDays(1).AddHours(2));

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/time-entries?projectId={projectOne.ProjectId}&fromUtc={Uri.EscapeDataString(baseTime.AddMinutes(-1).ToString("O"))}&toUtc={Uri.EscapeDataString(baseTime.AddHours(2).ToString("O"))}",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<TimeEntryListResponse>();

        result.Should().NotBeNull();
        result!.Items.Should().ContainSingle();
        result.Items[0].ProjectId.Should().Be(projectOne.ProjectId);
        result.Items[0].Description.Should().Be("Backend work");
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
            await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        loginResult.Should().NotBeNull();

        return new AuthResult(
            loginResult!.AccessToken,
            email);
    }

    private async Task<OrganizationResult> CreateOrganizationAsync(
        string accessToken,
        string name)
    {
        var slug =
            $"{name.Trim().ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}";

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            accessToken,
            null,
            new
            {
                name,
                slug
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return (await response.Content
            .ReadFromJsonAsync<OrganizationResult>())!;
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
                description = (string?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return (await response.Content
            .ReadFromJsonAsync<ProjectResult>())!;
    }

    private async Task<TaskResult> CreateTaskAsync(
        string accessToken,
        Guid organizationId,
        Guid projectId,
        string title)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/tasks",
            accessToken,
            organizationId,
            new
            {
                projectId,
                title,
                description = (string?)null,
                assigneeUserId = (Guid?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return (await response.Content
            .ReadFromJsonAsync<TaskResult>())!;
    }

    private async Task CreateManualTimeEntryAsync(
        AuthResult user,
        Guid organizationId,
        Guid projectId,
        string description,
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
                description,
                startedAtUtc = startedAt,
                endedAtUtc = endedAt
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<AcceptInvitationResponse> AcceptInvitationAsync(
        AuthResult owner,
        AuthResult member,
        Guid organizationId,
        string roleName)
    {
        var invitationResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organizationId,
            new
            {
                email = member.Email,
                roleName
            });

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invitation =
            await invitationResponse.Content
                .ReadFromJsonAsync<InvitationResponse>();

        invitation.Should().NotBeNull();

        var acceptResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations/accept",
            member.AccessToken,
            organizationId,
            new
            {
                token = invitation!.Token
            });

        acceptResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        return (await acceptResponse.Content
            .ReadFromJsonAsync<AcceptInvitationResponse>())!;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string uri,
        string accessToken,
        Guid? organizationId,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, uri);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

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
        string Email);

    private sealed record LoginResult(
        string AccessToken);

    private sealed record OrganizationResult(
        Guid OrganizationId,
        string Name);

    private sealed record ProjectResult(
        Guid Id,
        Guid OrganizationId,
        string Name,
        string Key,
        string? Description,
        string Status)
    {
        public Guid ProjectId => Id;
    }

    private sealed record TaskResult(
        Guid TaskId,
        Guid OrganizationId,
        Guid ProjectId,
        string Title,
        string? Description,
        Guid? AssigneeUserId,
        OpsFlow.Domain.Tasks.TaskStatus Status,
        string RowVersion);

    private sealed record TimeEntryResponse(
        Guid TimeEntryId,
        Guid OrganizationId,
        Guid UserId,
        Guid ProjectId,
        Guid? TaskId,
        string? Description,
        DateTimeOffset StartedAtUtc,
        DateTimeOffset? EndedAtUtc,
        long? DurationSeconds,
        bool IsManual,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? UpdatedAtUtc);

    private sealed record TimeEntryListResponse(
        IReadOnlyList<TimeEntryResponse> Items);

    private sealed record InvitationResponse(
        Guid InvitationId,
        string Email,
        string RoleName,
        DateTimeOffset ExpiresAtUtc,
        string Token);

    private sealed record AcceptInvitationResponse(
        Guid MembershipId,
        Guid OrganizationId,
        Guid UserId,
        Guid RoleId);
}
