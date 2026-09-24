namespace OpsFlow.Application.Features.Organizations.CreateOrganization;

public sealed record CreateOrganizationResult(
    Guid OrganizationId,
    string Name,
    string Slug);
