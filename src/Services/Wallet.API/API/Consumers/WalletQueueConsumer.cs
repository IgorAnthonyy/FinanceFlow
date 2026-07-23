using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wallet.API.API.Consumers;

/// <summary>
/// Consumidor de fila RabbitMQ responsável por processar eventos de carteira.
/// </summary>
public class WalletQueueConsumer : BaseRabbitMqSubscriber
{
    protected override string Queue => "wallet-service-queue";

    protected override IEnumerable<string> RoutingKeys => new[]
    {
        AppConstants.RabbitMq.RoutingKeys.UserCreated,
        AppConstants.RabbitMq.RoutingKeys.TransactionCreated
    };

    /// <summary>
    /// Construtor do consumidor de fila da carteira.
    /// </summary>
    /// <param name="persistentConnection">Conexão persistente com o RabbitMQ.</param>
    /// <param name="scopeFactory">Fábrica de escopo de serviços.</param>
    /// <param name="logger">Serviço de log.</param>
    public WalletQueueConsumer(
        IRabbitMqPersistentConnection persistentConnection,
        IServiceScopeFactory scopeFactory,
        ILogger<WalletQueueConsumer> logger)
        : base(persistentConnection, scopeFactory, logger)
    {
    }
}
