using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class MembersTests(
    OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanListOrganizationMembers()
    {
        var owner = await RegisterAndLoginAsync(
            $"members-owner-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Members Organization");

        var response = await SendAsync(
            HttpMethod.Get,
            "/api/v1/members",
            owner.AccessToken,
            organization.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<ListMembersResponse>();

        result.Should().NotBeNull();
        result!.Members.Should().ContainSingle();
        result.Members[0].UserId.Should().Be(owner.UserId);
        result.Members[0].IsActive.Should().BeTrue();
        result.Members[0].RoleName.Should().Be("Owner");
    }

    [Fact]
    public async Task OwnerCanInviteAndUserCanAcceptInvitation()
    {
        var owner = await RegisterAndLoginAsync(
            $"invite-owner-{Guid.NewGuid():N}@example.com");

        var invited = await RegisterAndLoginAsync(
            $"invite-user-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Invitation Organization");

        var inviteResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = invited.Email,
                roleName = "Member"
            });

        inviteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invitation =
            await inviteResponse.Content
                .ReadFromJsonAsync<InvitationResponse>();

        invitation.Should().NotBeNull();
        invitation!.OrganizationId.Should()
            .Be(organization.OrganizationId);
        invitation.Email.Should().Be(invited.Email);
        invitation.RoleName.Should().Be("Member");
        invitation.Token.Should().NotBeNullOrWhiteSpace();

        var acceptResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations/accept",
            invited.AccessToken,
            null,
            new
            {
                token = invitation.Token
            });

        acceptResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();
        accepted!.OrganizationId.Should()
            .Be(organization.OrganizationId);
        accepted.UserId.Should().Be(invited.UserId);
        accepted.MembershipId.Should().NotBe(Guid.Empty);
        accepted.RoleId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CannotCreateDuplicatePendingInvitation()
    {
        var owner = await RegisterAndLoginAsync(
            $"duplicate-owner-{Guid.NewGuid():N}@example.com");

        var invited = await RegisterAndLoginAsync(
            $"duplicate-user-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Duplicate Invitation Organization");

        var first = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = invited.Email,
                roleName = "Member"
            });

        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = invited.Email,
                roleName = "Member"
            });

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task InvitedUserCannotAcceptInvitationForDifferentUser()
    {
        var owner = await RegisterAndLoginAsync(
            $"wrong-invite-owner-{Guid.NewGuid():N}@example.com");

        var invited = await RegisterAndLoginAsync(
            $"wrong-invite-user-{Guid.NewGuid():N}@example.com");

        var wrongUser = await RegisterAndLoginAsync(
            $"wrong-invite-other-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Wrong User Invitation Organization");

        var inviteResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = invited.Email,
                roleName = "Member"
            });

        inviteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invitation =
            await inviteResponse.Content
                .ReadFromJsonAsync<InvitationResponse>();

        invitation.Should().NotBeNull();

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations/accept",
            wrongUser.AccessToken,
            null,
            new
            {
                token = invitation!.Token
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task OwnerCanChangeMemberRoleAndDeactivateReactivateMember()
    {
        var owner = await RegisterAndLoginAsync(
            $"manage-owner-{Guid.NewGuid():N}@example.com");

        var member = await RegisterAndLoginAsync(
            $"manage-member-{Guid.NewGuid():N}@example.com");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Member Management Organization");

        var inviteResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = member.Email,
                roleName = "Member"
            });

        inviteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invitation =
            await inviteResponse.Content
                .ReadFromJsonAsync<InvitationResponse>();

        invitation.Should().NotBeNull();

        var acceptResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations/accept",
            member.AccessToken,
            null,
            new
            {
                token = invitation!.Token
            });

        acceptResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();

        var membershipId = accepted!.MembershipId;

        var roleResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{membershipId}/role",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                roleName = "Project Manager"
            });

        roleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var roleResult =
            await roleResponse.Content
                .ReadFromJsonAsync<ChangeMemberRoleResponse>();

        roleResult.Should().NotBeNull();
        roleResult!.MembershipId.Should().Be(membershipId);
        roleResult.RoleName.Should().Be("Project Manager");
        roleResult.IsActive.Should().BeTrue();

        var deactivateResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{membershipId}/deactivate",
            owner.AccessToken,
            organization.OrganizationId);

        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deactivated =
            await deactivateResponse.Content
                .ReadFromJsonAsync<MemberStatusResponse>();

        deactivated.Should().NotBeNull();
        deactivated!.MembershipId.Should().Be(membershipId);
        deactivated.IsActive.Should().BeFalse();

        var reactivateResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{membershipId}/reactivate",
            owner.AccessToken,
            organization.OrganizationId);

        reactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reactivated =
            await reactivateResponse.Content
                .ReadFromJsonAsync<MemberStatusResponse>();

        reactivated.Should().NotBeNull();
        reactivated!.MembershipId.Should().Be(membershipId);
        reactivated.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task MembersAreIsolatedBetweenOrganizations()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"members-isolation-one-{Guid.NewGuid():N}@example.com");

        var ownerTwo = await RegisterAndLoginAsync(
            $"members-isolation-two-{Guid.NewGuid():N}@example.com");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "Members Isolation One");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Members Isolation Two");

        var inviteResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            ownerOne.AccessToken,
            organizationOne.OrganizationId,
            new
            {
                email = ownerTwo.Email,
                roleName = "Member"
            });

        inviteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await SendAsync(
            HttpMethod.Get,
            "/api/v1/members",
            ownerTwo.AccessToken,
            organizationTwo.OrganizationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<ListMembersResponse>();

        result.Should().NotBeNull();
        result!.Members.Should().ContainSingle();
        result.Members[0].UserId.Should().Be(ownerTwo.UserId);
    }

    [Fact]
    public async Task MembersRequireAuthentication()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/members");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

    private sealed record InvitationResponse(
        Guid InvitationId,
        Guid OrganizationId,
        string Email,
        string RoleName,
        DateTimeOffset ExpiresAtUtc,
        string Token);

    private sealed record AcceptInvitationResponse(
        Guid MembershipId,
        Guid OrganizationId,
        Guid UserId,
        Guid RoleId);

    private sealed record ChangeMemberRoleResponse(
        Guid MembershipId,
        Guid OrganizationId,
        Guid UserId,
        Guid RoleId,
        string RoleName,
        bool IsActive);

    private sealed record MemberStatusResponse(
        Guid MembershipId,
        Guid OrganizationId,
        Guid UserId,
        bool IsActive);
}
