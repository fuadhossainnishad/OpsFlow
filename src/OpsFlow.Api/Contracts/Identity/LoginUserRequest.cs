namespace OpsFlow.Api.Contracts.Identity;

public sealed record LoginUserRequest(
    string Email,
    string Password);
