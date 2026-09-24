namespace OpsFlow.Api.Contracts.Identity;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName);
