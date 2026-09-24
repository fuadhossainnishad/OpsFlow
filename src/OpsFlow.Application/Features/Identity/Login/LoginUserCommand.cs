namespace OpsFlow.Application.Features.Identity.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password);
