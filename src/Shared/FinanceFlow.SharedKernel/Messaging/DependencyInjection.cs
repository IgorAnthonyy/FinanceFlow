using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FinanceFlow.SharedKernel.Messaging;

/// <summary>
/// Classe responsável por adicionar o RabbitMQ ao container de serviços.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona o RabbitMQ ao container de serviços.
    /// </summary>
    /// <param name="services">O container de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <returns>O container de serviços.</returns>
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
        });

        services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
        {
            var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
            var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            return new RabbitMqPersistentConnection(connectionFactory, logger, options.RetryCount);
        });

        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        return services;
    }
}
