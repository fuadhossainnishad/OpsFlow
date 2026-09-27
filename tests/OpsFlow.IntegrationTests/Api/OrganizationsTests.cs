using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class OrganizationsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task UnauthenticatedUserCannotCreateOrganization()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/organizations",
            new
            {
                name = "Unauthorized Organization",
                slug = $"unauthorized-{Guid.NewGuid():N}"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthenticatedUserCanCreateOrganization()
    {
        var user = await RegisterAndLoginAsync(
            $"organization-create-{Guid.NewGuid():N}@example.com");

        var slug = $"opsflow-{Guid.NewGuid():N}";

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            user.AccessToken,
            null,
            new
            {
                name = "OpsFlow Organization",
                slug
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content
                .ReadFromJsonAsync<CreateOrganizationResponse>();

        result.Should().NotBeNull();
        result!.OrganizationId.Should().NotBe(Guid.Empty);
        result.Name.Should().Be("OpsFlow Organization");
        result.Slug.Should().Be(slug);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().Contain(result.OrganizationId.ToString());
    }

    [Fact]
    public async Task CreateOrganizationNormalizesSlug()
    {
        var user = await RegisterAndLoginAsync(
            $"organization-normalize-{Guid.NewGuid():N}@example.com");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            user.AccessToken,
            null,
            new
            {
                name = "  Normalized Organization  ",
                slug = $"  My-Organization-{Guid.NewGuid():N}  "
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content
                .ReadFromJsonAsync<CreateOrganizationResponse>();

        result.Should().NotBeNull();
        result!.Name.Should().Be("Normalized Organization");
        result.Slug.Should().StartWith("my-organization-");
        result.Slug.Should().NotContain(" ");
        result.Slug.Should().Be(result.Slug.ToLowerInvariant());
    }

    [Fact]
    public async Task DuplicateOrganizationSlugReturnsConflict()
    {
        var firstUser = await RegisterAndLoginAsync(
            $"organization-duplicate-one-{Guid.NewGuid():N}@example.com");

        var secondUser = await RegisterAndLoginAsync(
            $"organization-duplicate-two-{Guid.NewGuid():N}@example.com");

        var slug = $"duplicate-{Guid.NewGuid():N}";

        var firstResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            firstUser.AccessToken,
            null,
            new
            {
                name = "First Organization",
                slug
            });

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            secondUser.AccessToken,
            null,
            new
            {
                name = "Second Organization",
                slug
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DuplicateSlugWithDifferentCasingReturnsConflict()
    {
        var firstUser = await RegisterAndLoginAsync(
            $"organization-case-one-{Guid.NewGuid():N}@example.com");

        var secondUser = await RegisterAndLoginAsync(
            $"organization-case-two-{Guid.NewGuid():N}@example.com");

        var suffix = Guid.NewGuid().ToString("N");

        var firstResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            firstUser.AccessToken,
            null,
            new
            {
                name = "Case Organization",
                slug = $"Case-Sensitive-{suffix}"
            });

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            secondUser.AccessToken,
            null,
            new
            {
                name = "Another Case Organization",
                slug = $"case-sensitive-{suffix}"
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UserCanCreateMultipleOrganizations()
    {
        var user = await RegisterAndLoginAsync(
            $"organization-multiple-{Guid.NewGuid():N}@example.com");

        var firstResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            user.AccessToken,
            null,
            new
            {
                name = "First Organization",
                slug = $"first-{Guid.NewGuid():N}"
            });

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            user.AccessToken,
            null,
            new
            {
                name = "Second Organization",
                slug = $"second-{Guid.NewGuid():N}"
            });

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var first =
            await firstResponse.Content
                .ReadFromJsonAsync<CreateOrganizationResponse>();

        var second =
            await secondResponse.Content
                .ReadFromJsonAsync<CreateOrganizationResponse>();

        first.Should().NotBeNull();
        second.Should().NotBeNull();

        first!.OrganizationId.Should().NotBe(second!.OrganizationId);
        first.Slug.Should().NotBe(second.Slug);
    }

    [Fact]
    public async Task OrganizationCreationMakesCreatorAnOwner()
    {
        var user = await RegisterAndLoginAsync(
            $"organization-owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            user.AccessToken,
            "Owner Organization");

        var membersResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/members",
            user.AccessToken,
            organization.OrganizationId);

        membersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var members =
            await membersResponse.Content
                .ReadFromJsonAsync<ListMembersResponse>();

        members.Should().NotBeNull();
        members!.Members.Should().ContainSingle();

        var member = members.Members[0];

        member.UserId.Should().Be(user.UserId);
        member.RoleName.Should().Be("Owner");
        member.IsActive.Should().BeTrue();
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

    private async Task<CreateOrganizationResponse> CreateOrganizationAsync(
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
                .ReadFromJsonAsync<CreateOrganizationResponse>();

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

    private sealed record CreateOrganizationResponse(
        Guid OrganizationId,
        string Name,
        string Slug);

    private sealed record ListMembersResponse(
        IReadOnlyList<MemberResponse> Members);

    private sealed record MemberResponse(
        Guid MembershipId,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        Guid RoleId,
        string RoleName,
        bool IsActive,
        DateTimeOffset JoinedAtUtc);
}
