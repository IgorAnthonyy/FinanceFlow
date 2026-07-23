using Wallet.API.API.Extensions;
using Wallet.API.Extensions;
using Wallet.API.Infra.Data;
using Microsoft.EntityFrameworkCore;
using FinanceFlow.SharedKernel.Messaging;
using Wallet.API.API.Consumers;
using FinanceFlow.SharedKernel.Extensions;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services
    .AddOpenApi()
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddValidators()
    .AddServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddRabbitMqEventBus(builder.Configuration)
    .AddConfigureCors(builder.Configuration)
    .AddConfigureOpenTelemetry(builder.Configuration, "wallet-api");

builder.Services.AddHostedService<WalletQueueConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    context.Database.Migrate();
}

app.ConfigurePipeline();
app.Run();