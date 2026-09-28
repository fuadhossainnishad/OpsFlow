using Microsoft.AspNetCore.Authorization;
using OpsFlow.Api.Authorization;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Auditing.ListAuditLogs;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Api.Errors;
using OpsFlow.Infrastructure;
using OpsFlow.Infrastructure.Persistence;
using OpsFlow.Api.Endpoints;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Api.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpsFlow.Infrastructure.Security;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Api.Endpoints.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        var components = document.Components;
        components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Provide the access token as: Bearer {token}."
        };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, _) =>
    {
        var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (endpointMetadata?.OfType<IAllowAnonymous>().Any() == true)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document, null)] = []
        });

        return Task.CompletedTask;
    });
});
builder.Services.AddControllers();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddInfrastructure(builder.Configuration);


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SigningKey)),

            ValidateLifetime = true,

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    foreach (var permission in PermissionCodes.All)
    {
        options.AddPolicy(
            permission,
            policy => policy.Requirements.Add(
                new PermissionRequirement(permission)));
    }
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddDbContext<OpsFlowDbContext>(options =>
{
    var connectionString =
    builder.Configuration.GetConnectionString("OpsFlowDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'OpsFlowDatabase' was not found.");

    options.UseSqlServer(connectionString);
});


builder.Services.AddScoped<ListAuditLogsHandler>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseExceptionHandler();
app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    await httpContext.RequestServices
        .GetRequiredService<IProblemDetailsService>()
        .TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = httpContext.Response.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => "Bad Request",
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status403Forbidden => "Forbidden",
                    StatusCodes.Status404NotFound => "Not Found",
                    StatusCodes.Status405MethodNotAllowed => "Method Not Allowed",
                    StatusCodes.Status409Conflict => "Conflict",
                    _ => "Request failed"
                }
            }
        });
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();






app.MapControllers();

app.Run();

public partial class Program;
