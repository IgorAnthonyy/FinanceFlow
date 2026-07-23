using System.Text;
using System.Text.Json;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FinanceFlow.SharedKernel.Messaging;

/// <summary>
/// Classe responsável por publicar eventos no RabbitMQ.
/// </summary>
public class RabbitMqEventBus : IEventBus
{
    /// <summary>
    /// Conexão persistente com o RabbitMQ.
    /// </summary>
    private readonly IRabbitMqPersistentConnection _persistentConnection;
    /// <summary>
    /// Logger para registrar informações sobre a publicação de eventos.
    /// </summary>
    private readonly ILogger<RabbitMqEventBus> _logger;

    /// <summary>
    /// Construtor do RabbitMqEventBus.
    /// </summary>
    /// <param name="persistentConnection">Conexão persistente com o RabbitMQ.</param>
    /// <param name="logger">Logger para registrar informações sobre a publicação de eventos.</param>
    public RabbitMqEventBus(
        IRabbitMqPersistentConnection persistentConnection,
        ILogger<RabbitMqEventBus> logger)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Publica um evento.
    /// </summary>
    /// <typeparam name="T">O tipo do evento.</typeparam>
    /// <param name="@event">O evento a ser publicado.</param>
    /// <param name="routingKey">A chave de roteamento.</param>
    /// <param name="exchange">O exchange.</param>
    public async Task PublishAsync<T>(T @event, string routingKey, string exchange) where T : class
    {
        if (!string.IsNullOrEmpty(exchange))
            _logger.LogInformation("Publicando evento {EventName} no exchange '{Exchange}' com a routing key '{RoutingKey}'", @event.GetType().Name, exchange, routingKey);
        else
            _logger.LogInformation("Publicando evento {EventName} no exchange padrão com a routing key '{RoutingKey}'", @event.GetType().Name, routingKey);

        if (!_persistentConnection.IsConnected)
            await _persistentConnection.TryConnectAsync();

        using var channel = await _persistentConnection.CreateChannelAsync();

        if (!string.IsNullOrEmpty(exchange))
        {
            await channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);
        }

        var messageJson = JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = false });

        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: exchange ?? string.Empty,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}
