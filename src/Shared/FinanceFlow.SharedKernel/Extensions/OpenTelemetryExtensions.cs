using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FinanceFlow.SharedKernel.Extensions;

/// <summary>
/// Extensão centralizada para configurar OpenTelemetry em todos os serviços do FinanceFlow.
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Registra OpenTelemetry (Tracing e Métricas) com exportador OTLP (Jaeger/Collector).
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <param name="serviceName">Nome do serviço (ex: "identity-api").</param>
    /// <param name="isWorker">
    /// Quando true, omite a instrumentação ASP.NET Core (destinado ao Notifications.Worker).
    /// </param>
    public static IServiceCollection AddConfigureOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        bool isWorker = false)
    {
        var otlpEndpoint = configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";
        var environment  = configuration["OpenTelemetry:Environment"] ?? "development";

        var resource = ResourceBuilder
            .CreateDefault()
            .AddService(serviceName)
            .AddAttributes([new KeyValuePair<string, object>("deployment.environment", environment)]);

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resource)
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(opts => opts.SetDbStatementForText = true)
                    .AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint));

                if (!isWorker)
                    tracing.AddAspNetCoreInstrumentation();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resource)
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint));

                if (!isWorker)
                    metrics.AddAspNetCoreInstrumentation();
            });

        return services;
    }
}
