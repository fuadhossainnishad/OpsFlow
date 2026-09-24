using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Infrastructure.Persistence;
using OpsFlow.Infrastructure.Persistence.Repositories;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Infrastructure.Security;
using OpsFlow.Application.Features.Identity.RegisterUser;
using Microsoft.Extensions.Configuration;
using OpsFlow.Application.Features.Identity.Login;
using OpsFlow.Application.Features.Organizations.CreateOrganization;
using OpsFlow.Application.Features.Projects.CreateProject;
using OpsFlow.Application.Features.Tasks.CreateTask;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Features.Tasks.GetTask;

namespace OpsFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, OpsFlowUnitOfWork>();
        services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<RegisterUserHandler>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<LoginUserHandler>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        services.AddScoped<IMembershipRepository, MembershipRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<CreateOrganizationHandler>();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<CreateProjectHandler>();

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<GetTaskHandler>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();
        return services;
    }
}
