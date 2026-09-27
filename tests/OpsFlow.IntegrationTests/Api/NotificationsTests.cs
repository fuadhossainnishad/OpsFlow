using System.Net;
using OpsFlow.IntegrationTests.Infrastructure;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpsFlow.Domain.Notifications;
using OpsFlow.Infrastructure.Persistence;
using Xunit;

namespace OpsFlow.IntegrationTests.Api;

public sealed class NotificationsTests(OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task UserCanListUnreadNotificationGetUnreadCountAndMarkRead()
    {
        var auth = await RegisterAndLoginAsync($"notification-{Guid.NewGuid():N}@example.com");
        var organizationId = await CreateOrganizationAsync(auth.AccessToken);

        var notificationId = Guid.NewGuid();

        await SeedNotificationAsync(
            organizationId,
            auth.UserId,
            notificationId,
            "New task assigned",
            "You were assigned a task.");

        var list = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications?unreadOnly=true",
            auth.AccessToken,
            organizationId);

        list.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await list.Content.ReadFromJsonAsync<ListNotificationsResponse>();

        result.Should().NotBeNull();
        result!.Items.Should().ContainSingle();
        result.Items[0].Id.Should().Be(notificationId);
        result.Items[0].IsRead.Should().BeFalse();
        result.Items[0].Title.Should().Be("New task assigned");

        var unreadCount = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications/unread-count",
            auth.AccessToken,
            organizationId);

        unreadCount.StatusCode.Should().Be(HttpStatusCode.OK);
        (await unreadCount.Content.ReadFromJsonAsync<int>())
            .Should().Be(1);

        var markRead = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/notifications/{notificationId}/read",
            auth.AccessToken,
            organizationId);

        markRead.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var unreadCountAfterRead = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications/unread-count",
            auth.AccessToken,
            organizationId);

        unreadCountAfterRead.StatusCode.Should().Be(HttpStatusCode.OK);
        (await unreadCountAfterRead.Content.ReadFromJsonAsync<int>())
            .Should().Be(0);

        var allNotifications = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications?unreadOnly=false",
            auth.AccessToken,
            organizationId);

        allNotifications.StatusCode.Should().Be(HttpStatusCode.OK);

        var allResult =
            await allNotifications.Content.ReadFromJsonAsync<ListNotificationsResponse>();

        allResult.Should().NotBeNull();
        allResult!.Items.Should().ContainSingle();
        allResult.Items[0].Id.Should().Be(notificationId);
        allResult.Items[0].IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task NotificationsAreIsolatedByOrganizationAndRecipient()
    {
        var owner = await RegisterAndLoginAsync(
            $"notification-owner-{Guid.NewGuid():N}@example.com");

        var otherUser = await RegisterAndLoginAsync(
            $"notification-other-{Guid.NewGuid():N}@example.com");

        var organizationA = await CreateOrganizationAsync(owner.AccessToken);
        var organizationB = await CreateOrganizationAsync(owner.AccessToken);

        var organizationANotification = Guid.NewGuid();
        var organizationBNotification = Guid.NewGuid();
        var otherRecipientNotification = Guid.NewGuid();

        await SeedNotificationAsync(
            organizationA,
            owner.UserId,
            organizationANotification,
            "Organization A",
            "Visible in organization A.");

        await SeedNotificationAsync(
            organizationB,
            owner.UserId,
            organizationBNotification,
            "Organization B",
            "Visible in organization B.");

        await SeedNotificationAsync(
            organizationA,
            otherUser.UserId,
            otherRecipientNotification,
            "Other recipient",
            "Must not be visible to owner.");

        var organizationAResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications",
            owner.AccessToken,
            organizationA);

        organizationAResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var organizationAResult =
            await organizationAResponse.Content.ReadFromJsonAsync<ListNotificationsResponse>();

        organizationAResult.Should().NotBeNull();
        organizationAResult!.Items.Should().ContainSingle();
        organizationAResult.Items[0].Id.Should().Be(organizationANotification);

        var organizationBResponse = await SendAsync(
            HttpMethod.Get,
            "/api/v1/notifications",
            owner.AccessToken,
            organizationB);

        organizationBResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var organizationBResult =
            await organizationBResponse.Content.ReadFromJsonAsync<ListNotificationsResponse>();

        organizationBResult.Should().NotBeNull();
        organizationBResult!.Items.Should().ContainSingle();
        organizationBResult.Items[0].Id.Should().Be(organizationBNotification);
    }

    private async Task SeedNotificationAsync(
        Guid organizationId,
        Guid recipientUserId,
        Guid notificationId,
        string title,
        string message)
    {
        await using var scope = factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<OpsFlowDbContext>();

        var notification = Notification.Create(
            organizationId,
            recipientUserId,
            NotificationType.TaskCreated,
            title,
            message,
            "task",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        typeof(Notification)
            .GetProperty(nameof(Notification.Id))!
            .SetValue(notification, notificationId);

        dbContext.Notifications.Add(notification);

        await dbContext.SaveChangesAsync();
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

    private async Task<Guid> CreateOrganizationAsync(string accessToken)
    {
        var slug = $"notification-{Guid.NewGuid():N}";

        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            accessToken,
            null,
            new
            {
                name = $"Notification Organization {Guid.NewGuid():N}",
                slug
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content.ReadFromJsonAsync<OrganizationResponse>();

        result.Should().NotBeNull();

        return result!.OrganizationId;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string accessToken,
        Guid? organizationId,
        object? body = null)
    {
        var client = factory.CreateClient();

        using var request = new HttpRequestMessage(method, url);

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

        return await client.SendAsync(request);
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

    private sealed record ListNotificationsResponse(
        IReadOnlyList<NotificationResponse> Items,
        int Page,
        int PageSize);

    private sealed record NotificationResponse(
        Guid Id,
        Guid OrganizationId,
        Guid RecipientUserId,
        NotificationType Type,
        string Title,
        string? Message,
        string? ResourceType,
        Guid? ResourceId,
        bool IsRead,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? ReadAtUtc);
}
