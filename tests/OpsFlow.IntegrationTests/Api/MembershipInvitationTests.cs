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
}
