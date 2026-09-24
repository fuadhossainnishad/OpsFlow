namespace OpsFlow.Api.Contracts.Identity;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password);
