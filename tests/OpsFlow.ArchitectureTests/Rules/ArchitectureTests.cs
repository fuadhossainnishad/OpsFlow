using FluentAssertions;

namespace OpsFlow.ArchitectureTests.Rules;

public sealed class ArchitectureTests
{
    [Fact]
    public void DomainShouldNotReferenceApplicationInfrastructureOrApi()
    {
        var references = typeof(OpsFlow.Domain.Common.Entity).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        references.Should().NotContain("OpsFlow.Application");
        references.Should().NotContain("OpsFlow.Infrastructure");
        references.Should().NotContain("OpsFlow.Api");
    }

    [Fact]
    public void ApplicationShouldNotReferenceInfrastructureOrApi()
    {
        var references = typeof(OpsFlow.Application.Authorization.PermissionCodes).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        references.Should().NotContain("OpsFlow.Infrastructure");
        references.Should().NotContain("OpsFlow.Api");
    }

    [Fact]
    public void InfrastructureShouldNotReferenceApi()
    {
        var references = typeof(OpsFlow.Infrastructure.Persistence.OpsFlowDbContext).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        references.Should().NotContain("OpsFlow.Api");
    }

    [Fact]
    public void ApiShouldReferenceApplication()
    {
        var references = typeof(OpsFlow.Api.Controllers.OrganizationsController).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        references.Should().Contain("OpsFlow.Application");
    }
}
