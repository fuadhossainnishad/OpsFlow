using OpsFlow.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.Api.Contracts.Teams;

namespace OpsFlow.IntegrationTests.Api;

public sealed class TeamsTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanCreateListGetUpdateAndArchiveTeam()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Teams Organization");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Engineering",
                description = "Platform engineering"
            });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created =
            await createResponse.Content.ReadFromJsonAsync<TeamResponse>();

        created.Should().NotBeNull();

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/teams/{created!.TeamId}",
            owner.AccessToken,
            organization.OrganizationId);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var team =
            await getResponse.Content.ReadFromJsonAsync<TeamResponse>();

        team!.Name.Should().Be("Engineering");

        var updateResponse = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/teams/{created.TeamId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Platform Engineering",
                description = "Core platform team"
            });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var archiveResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/teams/{created.TeamId}/archive",
            owner.AccessToken,
            organization.OrganizationId);

        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updateArchivedResponse = await SendAsync(
            HttpMethod.Patch,
            $"/api/v1/teams/{created.TeamId}",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Should Fail",
                description = "Archived"
            });

        updateArchivedResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DuplicateTeamNameIsRejected()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Duplicate Team Organization");

        var first = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Engineering",
                description = (string?)null
            });

        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = " engineering ",
                description = (string?)null
            });

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task OwnerCanAddMemberAndAssignTeamLead()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Team Membership Organization");

        var membership = await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Backend",
                description = "Backend team"
            });

        var team =
            await createResponse.Content.ReadFromJsonAsync<TeamResponse>();

        var addResponse = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/teams/{team!.TeamId}/members",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var leadResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/teams/{team.TeamId}/lead",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        leadResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/teams/{team.TeamId}",
            owner.AccessToken,
            organization.OrganizationId);

        var result =
            await getResponse.Content.ReadFromJsonAsync<TeamResponse>();

        result!.TeamLeadMembershipId.Should().Be(membership.MembershipId);
    }

    [Fact]
    public async Task DuplicateTeamMemberIsRejected()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Duplicate Team Member Organization");

        var membership = await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Engineering",
                description = (string?)null
            });

        var team =
            await createResponse.Content.ReadFromJsonAsync<TeamResponse>();

        var first = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/teams/{team!.TeamId}/members",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/teams/{team.TeamId}/members",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task TeamLeadCannotBeRemoved()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Team Lead Organization");

        var membership = await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Engineering",
                description = (string?)null
            });

        var team =
            await createResponse.Content.ReadFromJsonAsync<TeamResponse>();

        await SendAsync(
            HttpMethod.Post,
            $"/api/v1/teams/{team!.TeamId}/members",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        await SendAsync(
            HttpMethod.Put,
            $"/api/v1/teams/{team.TeamId}/lead",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                membershipId = membership.MembershipId
            });

        var response = await SendAsync(
            HttpMethod.Delete,
            $"/api/v1/teams/{team.TeamId}/members/{membership.MembershipId}",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RegularMemberCannotManageTeams()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Team Authorization Organization");

        await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            member.AccessToken,
            organization.OrganizationId,
            new
            {
                name = "Unauthorized Team",
                description = (string?)null
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TeamCannotBeAccessedFromAnotherOrganization()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"owner-one-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var ownerTwo = await RegisterAndLoginAsync(
            $"owner-two-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "First Team Organization");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Second Team Organization");

        var createResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/teams",
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId,
            new
            {
                name = "Private Team",
                description = (string?)null
            });

        var team =
            await createResponse.Content.ReadFromJsonAsync<TeamResponse>();

        var response = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/teams/{team!.TeamId}",
            ownerOne.AccessToken,
            organizationOne.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(
        string email,
        string password)
    {
        var registerResponse = await factory.CreateClient().PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Test",
                lastName = "User"
            });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await factory.CreateClient().PostAsJsonAsync(
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
        var slug = $"{name.Trim().ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}";

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

        return (await response.Content.ReadFromJsonAsync<OrganizationResult>())!;
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
        string Email);

    private sealed record LoginResult(
        string AccessToken);

    private sealed record OrganizationResult(
        Guid OrganizationId,
        string Name);

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
