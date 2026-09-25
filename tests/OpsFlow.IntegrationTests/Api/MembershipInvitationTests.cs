using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using OpsFlow.IntegrationTests.Infrastructure;

namespace OpsFlow.IntegrationTests.Api;

public sealed class MembershipInvitationTests
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MembershipInvitationTests(
        OpsFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OwnerCanInviteUserAndUserCanAcceptInvitation()
    {
        var ownerEmail =
            $"owner-{Guid.NewGuid():N}@example.com";

        var memberEmail =
            $"member-{Guid.NewGuid():N}@example.com";

        var password = "Password123!";

        var owner = await RegisterAndLoginAsync(
            ownerEmail,
            password);

        var member = await RegisterAndLoginAsync(
            memberEmail,
            password);

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Membership Organization");

        var invitationResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = memberEmail,
                roleName = "Member"
            });

        invitationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var invitation =
            await invitationResponse.Content
                .ReadFromJsonAsync<InvitationResponse>();

        invitation.Should().NotBeNull();
        invitation!.OrganizationId
            .Should()
            .Be(organization.OrganizationId);

        invitation.Email
            .Should()
            .Be(memberEmail);

        invitation.RoleName
            .Should()
            .Be("Member");

        invitation.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        var acceptResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations/accept",
            member.AccessToken,
            null,
            new
            {
                token = invitation.Token
            });

        acceptResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();

        accepted!.OrganizationId
            .Should()
            .Be(organization.OrganizationId);

        accepted.UserId
            .Should()
            .Be(member.UserId);

        var membersResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/members",
            owner.AccessToken,
            organization.OrganizationId);

        membersResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DuplicatePendingInvitationShouldBeRejected()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Duplicate Invitation Organization");

        var firstResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = member.Email,
                roleName = "Member"
            });

        firstResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var secondResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = member.Email,
                roleName = "Member"
            });

        secondResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task OwnerCanChangeMemberRole()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Role Management Organization");

        var invitationResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = member.Email,
                roleName = "Member"
            });

        invitationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var invitation =
            await invitationResponse.Content
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

        acceptResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();

        var changeRoleResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{accepted!.MembershipId}/role",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                roleName = "Team Lead"
            });

        changeRoleResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var changed =
            await changeRoleResponse.Content
                .ReadFromJsonAsync<ChangeMemberRoleResponse>();

        changed.Should().NotBeNull();
        changed!.MembershipId
            .Should()
            .Be(accepted.MembershipId);

        changed.RoleName
            .Should()
            .Be("Team Lead");

        changed.IsActive
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task OwnerCanDeactivateAndReactivateMember()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Member Lifecycle Organization");

        var invitationResponse = await SendAsync(
            HttpMethod.Post,
            "/api/v1/members/invitations",
            owner.AccessToken,
            organization.OrganizationId,
            new
            {
                email = member.Email,
                roleName = "Member"
            });

        invitationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var invitation =
            await invitationResponse.Content
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

        acceptResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();

        var deactivateResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{accepted!.MembershipId}/deactivate",
            owner.AccessToken,
            organization.OrganizationId);

        deactivateResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var deactivated =
            await deactivateResponse.Content
                .ReadFromJsonAsync<MemberStatusResponse>();

        deactivated.Should().NotBeNull();
        deactivated!.MembershipId
            .Should()
            .Be(accepted.MembershipId);

        deactivated.IsActive
            .Should()
            .BeFalse();

        var reactivateResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{accepted.MembershipId}/reactivate",
            owner.AccessToken,
            organization.OrganizationId);

        reactivateResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var reactivated =
            await reactivateResponse.Content
                .ReadFromJsonAsync<MemberStatusResponse>();

        reactivated.Should().NotBeNull();
        reactivated!.MembershipId
            .Should()
            .Be(accepted.MembershipId);

        reactivated.IsActive
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task RegularMemberCannotManageOrganizationMembers()
    {
        var owner = await RegisterAndLoginAsync(
            $"owner-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var target = await RegisterAndLoginAsync(
            $"target-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organization = await CreateOrganizationAsync(
            owner.AccessToken,
            "Member Authorization Organization");

        await AcceptInvitationAsync(
            owner,
            member,
            organization.OrganizationId,
            "Member");

        var targetMembership = await AcceptInvitationAsync(
            owner,
            target,
            organization.OrganizationId,
            "Member");

        var changeRoleResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{targetMembership.MembershipId}/role",
            member.AccessToken,
            organization.OrganizationId,
            new
            {
                roleName = "Team Lead"
            });

        changeRoleResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);

        var deactivateResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{targetMembership.MembershipId}/deactivate",
            member.AccessToken,
            organization.OrganizationId);

        deactivateResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task MemberCannotManipulateMembershipFromAnotherOrganization()
    {
        var ownerOne = await RegisterAndLoginAsync(
            $"owner-one-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var ownerTwo = await RegisterAndLoginAsync(
            $"owner-two-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var member = await RegisterAndLoginAsync(
            $"member-{Guid.NewGuid():N}@example.com",
            "Password123!");

        var organizationOne = await CreateOrganizationAsync(
            ownerOne.AccessToken,
            "First Isolation Organization");

        var organizationTwo = await CreateOrganizationAsync(
            ownerTwo.AccessToken,
            "Second Isolation Organization");

        var membership =
            await AcceptInvitationAsync(
                ownerTwo,
                member,
                organizationTwo.OrganizationId,
                "Member");

        var response = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/members/{membership.MembershipId}/role",
            ownerOne.AccessToken,
            organizationOne.OrganizationId,
            new
            {
                roleName = "Team Lead"
            });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
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

        invitationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var invitation =
            await invitationResponse.Content
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

        acceptResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var accepted =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptInvitationResponse>();

        accepted.Should().NotBeNull();

        return accepted!;
    }

    private async Task<AuthResult> RegisterAndLoginAsync(
        string email,
        string password)
    {
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Membership",
                lastName = "Test"
            });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        loginResponse.EnsureSuccessStatusCode();

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        login.Should().NotBeNull();

        using var meRequest =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/api/v1/auth/me");

        meRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login!.AccessToken);

        var meResponse =
            await _client.SendAsync(meRequest);

        meResponse.EnsureSuccessStatusCode();

        var me =
            await meResponse.Content
                .ReadFromJsonAsync<CurrentUserResponse>();

        me.Should().NotBeNull();

        return new AuthResult(
            login.AccessToken,
            me!.UserId,
            email);
    }

    private async Task<OrganizationResponse> CreateOrganizationAsync(
        string token,
        string name)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            token,
            null,
            new
            {
                name,
                slug =
                    $"{name.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid():N}"
            });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var organization =
            await response.Content
                .ReadFromJsonAsync<OrganizationResponse>();

        organization.Should().NotBeNull();

        return organization!;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string token,
        Guid? organizationId,
        object? body = null)
    {
        using var request =
            new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

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

        return await _client.SendAsync(request);
    }

    private sealed record AuthResult(
        string AccessToken,
        Guid UserId,
        string Email);

    private sealed record LoginResponse(
        string AccessToken,
        string RefreshToken);

    private sealed record CurrentUserResponse(
        Guid UserId);

    private sealed record OrganizationResponse(
        Guid OrganizationId,
        string Name,
        string Slug);

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
