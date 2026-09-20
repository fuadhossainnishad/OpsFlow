namespace OpsFlow.Application.Features.Identity.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password);
