using Microsoft.EntityFrameworkCore;
using FinanceFlow.SharedKernel.Extensions;
using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Notifications.Worker.Domain.Interfaces;
using Notifications.Worker.Application.Services;
using Notifications.Worker.Infra.Data;
using Notifications.Worker.Infra.Repositories;
using Notifications.Worker.Infra.Services;
using Notifications.Worker.Consumers;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationsLogRepository, NotificationsLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddRabbitMqEventBus(builder.Configuration);

builder.Services.AddConfigureOpenTelemetry(
    builder.Configuration, "notifications-worker", isWorker: true);

builder.Services.AddScoped<IMessageBrokerClientApplicationService, NotificationApplicationService>();

builder.Services.AddHostedService<NotificationQueueConsumer>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    context.Database.Migrate();
}

host.Run();
