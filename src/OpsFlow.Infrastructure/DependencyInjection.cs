using Microsoft.Extensions.DependencyInjection;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Infrastructure.Persistence.Repositories;

namespace OpsFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
