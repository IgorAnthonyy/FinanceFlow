using FinanceFlow.SharedKernel.Extensions;
using FinanceFlow.SharedKernel.Messaging;
using Identity.API.Extensions;
using Identity.API.Infra.Data;
using Microsoft.EntityFrameworkCore;

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
    .AddConfigureOpenTelemetry(builder.Configuration, "identity-api");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    context.Database.Migrate();
}

app.ConfigurePipeline();
app.Run();