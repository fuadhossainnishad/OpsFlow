namespace OpsFlow.Api.Contracts.Organizations;

public sealed record CreateOrganizationResponse(
    Guid OrganizationId,
    string Name,
    string Slug);
