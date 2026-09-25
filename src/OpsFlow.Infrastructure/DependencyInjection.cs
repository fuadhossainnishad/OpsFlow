using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpsFlow.Infrastructure.Persistence;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Infrastructure.Security;
using OpsFlow.Application.Features.Identity.RegisterUser;
using Microsoft.Extensions.Configuration;
using OpsFlow.Application.Features.Identity.Login;
using OpsFlow.Application.Features.Organizations.CreateOrganization;
using OpsFlow.Application.Features.Projects.CreateProject;
using OpsFlow.Application.Features.Projects.ArchiveProject;
using OpsFlow.Application.Features.Projects.UpdateProject;
using OpsFlow.Application.Features.Projects.GetProject;
using OpsFlow.Application.Features.Projects.ListProjects;
using OpsFlow.Application.Features.Tasks.CreateTask;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Features.Tasks.GetTask;
using OpsFlow.Application.Features.Tasks.UpdateTask;
using OpsFlow.Application.Features.Tasks.AssignTask;
using OpsFlow.Application.Features.Tasks.ChangeTaskStatus;
using OpsFlow.Application.Features.Members.InviteMember;
using OpsFlow.Application.Features.Members.AcceptInvitation;
using OpsFlow.Application.Features.Members.ListMembers;
using OpsFlow.Application.Features.Members.ChangeMemberRole;
using OpsFlow.Application.Features.Members.DeactivateMember;
using OpsFlow.Application.Features.Members.ReactivateMember;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Application.Features.Teams.AddTeamMember;
using OpsFlow.Application.Features.Teams.ArchiveTeam;
using OpsFlow.Application.Features.Teams.ChangeTeamLead;
using OpsFlow.Application.Features.Teams.CreateTeam;
using OpsFlow.Application.Features.Teams.GetTeam;
using OpsFlow.Application.Features.Teams.ListTeams;
using OpsFlow.Application.Features.Teams.RemoveTeamMember;
using OpsFlow.Application.Features.Teams.UpdateTeam;
using OpsFlow.Infrastructure.Persistence.Repositories;
using OpsFlow.Application.Features.Projects;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Infrastructure.Auditing;

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
        services.AddScoped<IOrganizationInvitationRepository, OrganizationInvitationRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<CreateOrganizationHandler>();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<CreateProjectHandler>();
        services.AddScoped<ListProjectsHandler>();
        services.AddScoped<GetProjectHandler>();
        services.AddScoped<UpdateProjectHandler>();
        services.AddScoped<ArchiveProjectHandler>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<GetTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<AssignTaskHandler>();
        services.AddScoped<ChangeTaskStatusHandler>();
        services.AddScoped<InviteMemberHandler>();
        services.AddScoped<AcceptInvitationHandler>();
        services.AddScoped<ListMembersHandler>();
        services.AddScoped<ChangeMemberRoleHandler>();
        services.AddScoped<DeactivateMemberHandler>();
        services.AddScoped<ReactivateMemberHandler>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<CreateTeamHandler>();
        services.AddScoped<ListTeamsHandler>();
        services.AddScoped<GetTeamHandler>();
        services.AddScoped<UpdateTeamHandler>();
        services.AddScoped<ArchiveTeamHandler>();
        services.AddScoped<AddTeamMemberHandler>();
        services.AddScoped<RemoveTeamMemberHandler>();
        services.AddScoped<ChangeTeamLeadHandler>();

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddHttpContextAccessor();

        return services;
    }
}
