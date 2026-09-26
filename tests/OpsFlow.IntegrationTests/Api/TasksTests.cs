using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using DomainTaskStatus = OpsFlow.Domain.Tasks.TaskStatus;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class TasksTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateAndRetrieveTask()
    {
        var owner = await RegisterAndLoginAsync(
            $"task-owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Task Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "TASK");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/tasks",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                projectId = project.Id,
                title = "Implement authentication",
                description = "Build JWT authentication flow",
                assigneeUserId = (Guid?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created =
            await response.Content.ReadFromJsonAsync<TaskResponse>();

        created.Should().NotBeNull();
        created!.ProjectId.Should().Be(project.Id);
        created.Title.Should().Be("Implement authentication");
        created.Description.Should().Be("Build JWT authentication flow");
        created.Status.Should().Be(DomainTaskStatus.Todo);
        created.RowVersion.Should().NotBeNullOrWhiteSpace();

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/tasks/{created.TaskId}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrieved =
            await getResponse.Content.ReadFromJsonAsync<TaskDetailsResponse>();

        retrieved.Should().NotBeNull();
        retrieved!.TaskId.Should().Be(created.TaskId);
        retrieved.Title.Should().Be("Implement authentication");
        retrieved.Status.Should().Be(DomainTaskStatus.Todo);
    }

    [Fact]
    public async Task OwnerCanUpdateTask()
    {
        var owner = await RegisterAndLoginAsync(
            $"task-update-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Task Update Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "UPD");

        var task = await CreateTaskAsync(
            owner.AccessToken,
            organization.OrganizationId,
            project.Id,
            "Original title");

        var response = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/tasks/{task.TaskId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                title = "Updated title",
                description = "Updated description",
                rowVersion = task.RowVersion
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated =
            await response.Content.ReadFromJsonAsync<TaskResponse>();

        updated.Should().NotBeNull();
        updated!.TaskId.Should().Be(task.TaskId);
        updated.Title.Should().Be("Updated title");
        updated.Description.Should().Be("Updated description");
        updated.RowVersion.Should().NotBe(task.RowVersion);
    }

    [Fact]
    public async Task OwnerCanAssignTaskAndChangeStatus()
    {
        var owner = await RegisterAndLoginAsync(
            $"task-status-{Guid.NewGuid():N}@example.com");

        var assignee = await RegisterAndLoginAsync(
            $"task-assignee-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Task Status Organization");

        await AcceptInvitationAsync(
            owner,
            assignee,
            organization.OrganizationId,
            "Member");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "STAT");

        var task = await CreateTaskAsync(
            owner.AccessToken,
            organization.OrganizationId,
            project.Id,
            "Assigned task");

        var assignResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/tasks/{task.TaskId}/assignee",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                assigneeUserId = GetUserId(assignee),
                rowVersion = task.RowVersion
            });

        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var assigned =
            await assignResponse.Content.ReadFromJsonAsync<TaskResponse>();

        assigned.Should().NotBeNull();
        assigned!.AssigneeUserId.Should().Be(GetUserId(assignee));

        var statusResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/tasks/{task.TaskId}/status",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                status = DomainTaskStatus.InProgress,
                rowVersion = assigned.RowVersion
            });

        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var changed =
            await statusResponse.Content.ReadFromJsonAsync<TaskStatusResponse>();

        changed.Should().NotBeNull();
        changed!.TaskId.Should().Be(task.TaskId);
        changed.Status.Should().Be(DomainTaskStatus.InProgress);
    }

    [Fact]
    public async Task TaskUpdateWithStaleRowVersionReturnsConflict()
    {
        var owner = await RegisterAndLoginAsync(
            $"task-concurrency-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Task Concurrency Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "CON");

        var task = await CreateTaskAsync(
            owner.AccessToken,
            organization.OrganizationId,
            project.Id,
            "Concurrency task");

        var firstUpdate = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/tasks/{task.TaskId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                title = "First update",
                description = "First",
                rowVersion = task.RowVersion
            });

        firstUpdate.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondUpdate = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/tasks/{task.TaskId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                title = "Stale update",
                description = "Should fail",
                rowVersion = task.RowVersion
            });

        secondUpdate.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UserCannotAccessTaskFromAnotherOrganization()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"task-org-one-{Guid.NewGuid():N}@example.com");

        var ownerTwo = await RegisterAndLoginAsync(
            $"task-org-two-{Guid.NewGuid():N}@example.com");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "First Task Organization");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Second Task Organization");

        var project = await CreateProjectAsync(
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            "Private",
            "PRIVATE");

        var task = await CreateTaskAsync(
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            project.Id,
            "Private task");

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/tasks/{task.TaskId}",
            ownerOne.AccessToken,
            organizationOne.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
            new { email, password });

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
                slug = $"{name.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid():N}"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content.ReadFromJsonAsync<OrganizationResponse>();

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
                description = "Task integration project"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content.ReadFromJsonAsync<ProjectResponse>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task<TaskResponse> CreateTaskAsync(
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
                description = "Task integration test",
                assigneeUserId = (Guid?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content.ReadFromJsonAsync<TaskResponse>();

        result.Should().NotBeNull();

        return result!;
    }

    private async Task AcceptInvitationAsync(
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

    private static Guid GetUserId(AuthResult auth) => auth.UserId;

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
        Guid Id);

    private sealed record TaskResponse(
        Guid TaskId,
        Guid OrganizationId,
        Guid ProjectId,
        string Title,
        string? Description,
        Guid? AssigneeUserId,
        DomainTaskStatus Status,
        string RowVersion);

    private sealed record TaskDetailsResponse(
        Guid TaskId,
        Guid OrganizationId,
        Guid ProjectId,
        string Title,
        string? Description,
        Guid? AssigneeUserId,
        DomainTaskStatus Status,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? UpdatedAtUtc,
        string RowVersion);

    private sealed record TaskStatusResponse(
        Guid TaskId,
        Guid OrganizationId,
        DomainTaskStatus Status,
        string RowVersion);

    private sealed record InvitationResponse(
        string Token);
}
