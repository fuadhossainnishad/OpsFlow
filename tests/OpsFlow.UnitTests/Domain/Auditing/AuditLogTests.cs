using FluentAssertions;
using OpsFlow.Domain.Auditing;

namespace OpsFlow.UnitTests.Domain.Auditing;

public sealed class AuditLogTests
{
    [Fact]
    public void CreateShouldInitializeAuditLog()
    {
        var organizationId = Guid.NewGuid();
        var actorUserId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();

        var log = AuditLog.Create(
            organizationId,
            actorUserId,
            "  Created  ",
            "  Project  ",
            resourceId,
            " 127.0.0.1 ",
            " Mozilla/5.0 ",
            " correlation-123 ",
            """{"name":"old"}""",
            """{"name":"new"}""");

        log.Id.Should().NotBe(Guid.Empty);
        log.OrganizationId.Should().Be(organizationId);
        log.ActorUserId.Should().Be(actorUserId);
        log.Action.Should().Be("Created");
        log.Resource.Should().Be("Project");
        log.ResourceId.Should().Be(resourceId);
        log.IpAddress.Should().Be("127.0.0.1");
        log.UserAgent.Should().Be("Mozilla/5.0");
        log.CorrelationId.Should().Be("correlation-123");
        log.BeforeJson.Should().Be("""{"name":"old"}""");
        log.AfterJson.Should().Be("""{"name":"new"}""");
        log.OccurredAtUtc.Should().BeCloseTo(
            DateTimeOffset.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CreateShouldNormalizeEmptyActorIdToNull()
    {
        var log = AuditLog.Create(
            Guid.NewGuid(),
            Guid.Empty,
            "Created",
            "Project");

        log.ActorUserId.Should().BeNull();
    }

    [Fact]
    public void CreateShouldAllowNullActor()
    {
        var log = AuditLog.Create(
            Guid.NewGuid(),
            null,
            "Created",
            "Project");

        log.ActorUserId.Should().BeNull();
    }

    [Fact]
    public void CreateShouldPreserveOptionalNullValues()
    {
        var log = AuditLog.Create(
            Guid.NewGuid(),
            null,
            "Created",
            "Project");

        log.ResourceId.Should().BeNull();
        log.IpAddress.Should().BeNull();
        log.UserAgent.Should().BeNull();
        log.CorrelationId.Should().BeNull();
        log.BeforeJson.Should().BeNull();
        log.AfterJson.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => AuditLog.Create(
            Guid.Empty,
            null,
            "Created",
            "Project");

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Organization ID is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankAction(string action)
    {
        var act = () => AuditLog.Create(
            Guid.NewGuid(),
            null,
            action,
            "Project");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankResource(string resource)
    {
        var action = () => AuditLog.Create(
            Guid.NewGuid(),
            null,
            "Created",
            resource);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldTrimNullableMetadata()
    {
        var log = AuditLog.Create(
            Guid.NewGuid(),
            null,
            "Created",
            "Project",
            ipAddress: "  10.0.0.1  ",
            userAgent: "  Test Agent  ",
            correlationId: "  abc-123  ");

        log.IpAddress.Should().Be("10.0.0.1");
        log.UserAgent.Should().Be("Test Agent");
        log.CorrelationId.Should().Be("abc-123");
    }
}
