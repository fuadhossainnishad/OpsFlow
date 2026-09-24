using Microsoft.AspNetCore.Authorization;
using OpsFlow.Api.Authorization;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Authorization;
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddProblemDetails();
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


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();






app.MapControllers();

app.Run();
