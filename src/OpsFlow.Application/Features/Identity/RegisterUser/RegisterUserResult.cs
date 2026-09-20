namespace OpsFlow.Application.Features.Identity.RegisterUser;

public sealed record RegisterUserResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName);
