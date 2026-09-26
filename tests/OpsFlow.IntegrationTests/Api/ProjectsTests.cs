using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class ProjectsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateRetrieveAndListProject()
    {
        var owner = await RegisterAndLoginAsync(
            $"project-owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Projects Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLT");

        project.OrganizationId.Should().Be(organization.OrganizationId);
        project.Name.Should().Be("Platform");
        project.Key.Should().Be("PLT");
        project.Description.Should().Be("Project integration test");
        project.Status.Should().Be("Active");

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/projects/{project.Id}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrieved =
            await getResponse.Content
                .ReadFromJsonAsync<ProjectResponse>();

        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(project.Id);
        retrieved.OrganizationId.Should().Be(organization.OrganizationId);
        retrieved.Name.Should().Be("Platform");
        retrieved.Key.Should().Be("PLT");

        var listResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/projects",
            owner.AccessToken,
            organization.OrganizationId);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects =
            await listResponse.Content
                .ReadFromJsonAsync<List<ListProjectResponse>>();

        projects.Should().NotBeNull();
        projects.Should().ContainSingle(
            projectItem => projectItem.Id == project.Id);
    }

    [Fact]
    public async Task OwnerCanUpdateProject()
    {
        var owner = await RegisterAndLoginAsync(
            $"project-update-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Project Update Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLT");

        var response = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/projects/{project.Id}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Operations Platform",
                description = "Updated project description"
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated =
            await response.Content
                .ReadFromJsonAsync<ProjectResponse>();

        updated.Should().NotBeNull();
        updated!.Id.Should().Be(project.Id);
        updated.OrganizationId.Should().Be(organization.OrganizationId);
        updated.Name.Should().Be("Operations Platform");
        updated.Key.Should().Be("PLT");
        updated.Description.Should().Be("Updated project description");
        updated.Status.Should().Be("Active");
    }

    [Fact]
    public async Task OwnerCanArchiveProject()
    {
        var owner = await RegisterAndLoginAsync(
            $"project-archive-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Project Archive Organization");

        var project = await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLT");

        var archiveResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/projects/{project.Id}/archive",
            owner.AccessToken,
            organization.OrganizationId);

        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/projects/{project.Id}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var archived =
            await getResponse.Content
                .ReadFromJsonAsync<ProjectResponse>();

        archived.Should().NotBeNull();
        archived!.Id.Should().Be(project.Id);
        archived.Status.Should().Be("Archived");
    }

    [Fact]
    public async Task DuplicateProjectKeyReturnsConflict()
    {
        var owner = await RegisterAndLoginAsync(
            $"project-duplicate-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Project Duplicate Organization");

        await CreateProjectAsync(
            owner.AccessToken,
            organization.OrganizationId,
            "Platform",
            "PLT");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/projects",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Another Platform",
                key = "PLT",
                description = "Duplicate key"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UserCannotAccessProjectFromAnotherOrganization()
    {
        var ownerA = await RegisterAndLoginAsync(
            $"project-org-a-{Guid.NewGuid():N}@example.com");

        var ownerB = await RegisterAndLoginAsync(
            $"project-org-b-{Guid.NewGuid():N}@example.com");

        var organizationA = await CreateOrganizationAsync(
            ownerA.AccessToken,
            "Project Organization A");

        var organizationB = await CreateOrganizationAsync(
            ownerB.AccessToken,
            "Project Organization B");

        var project = await CreateProjectAsync(
            ownerA.AccessToken,
            organizationA.OrganizationId,
            "Private Project",
            "PRV");

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/projects/{project.Id}",
            ownerB.AccessToken,
            organizationB.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
        string? Description,
        string Status);

    private sealed record ListProjectResponse(
        Guid Id,
        string Name,
        string Key,
        string? Description,
        string Status);

    private sealed record ListProjectResponseWrapper(
        List<ListProjectResponse> Projects);
}
