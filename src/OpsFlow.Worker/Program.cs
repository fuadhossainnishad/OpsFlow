using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Infrastructure.Messaging;
using OpsFlow.Worker;
using OpsFlow.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
