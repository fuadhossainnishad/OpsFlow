using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Api.Contracts.Identity;

public sealed record LoginUserRequest(
    [EmailAddress]
    string Email,
    string Password);
