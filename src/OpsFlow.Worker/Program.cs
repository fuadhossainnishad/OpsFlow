using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Application.Features.Tasks.Events;
using OpsFlow.Infrastructure.Messaging;
using OpsFlow.Infrastructure.Persistence;
using OpsFlow.Worker;
using OpsFlow.Worker.Messaging;
using OpsFlow.Worker.Outbox;

var builder = Host.CreateApplicationBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("OpsFlowDatabase")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:OpsFlowDatabase is not configured.");

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddDbContext<OpsFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddScoped<OutboxDispatcher>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
