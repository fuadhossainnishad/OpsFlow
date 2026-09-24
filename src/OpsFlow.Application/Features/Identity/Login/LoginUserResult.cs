namespace OpsFlow.Application.Features.Identity.Login;

public sealed record LoginUserResult(
    Guid UserId,
    string AccessToken,
    string RefreshToken);
