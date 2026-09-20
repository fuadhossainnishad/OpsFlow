using Microsoft.EntityFrameworkCore;
using OpsFlow.Infrastructure;
using OpsFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();

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

app.UseHttpsRedirection();

app.Run();
