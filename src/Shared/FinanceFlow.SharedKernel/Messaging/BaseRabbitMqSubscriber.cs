using System.Text;
using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinanceFlow.SharedKernel.Messaging;

public abstract class BaseRabbitMqSubscriber : BackgroundService
{
    private readonly IRabbitMqPersistentConnection _persistentConnection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BaseRabbitMqSubscriber> _logger;
    private IChannel _channel;

    /// <summary>
    /// Nome da fila.
    /// </summary>
    protected abstract string Queue { get; }

    /// <summary>
    /// Chaves de roteamento.
    /// </summary>
    protected abstract IEnumerable<string> RoutingKeys { get; }

    /// <summary>
    /// Construtor do BaseRabbitMqSubscriber.
    /// </summary>
    /// <param name="persistentConnection">Conexão persistente com o RabbitMQ.</param>
    /// <param name="scopeFactory">Fábrica de escopos de serviços.</param>
    /// <param name="logger">Logger para registrar informações sobre a conexão com o RabbitMQ.</param>
    protected BaseRabbitMqSubscriber(
        IRabbitMqPersistentConnection persistentConnection,
        IServiceScopeFactory scopeFactory,
        ILogger<BaseRabbitMqSubscriber> logger)
    {
        _persistentConnection = persistentConnection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executa a lógica do subscriber.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelamento.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriberName = GetType().Name;
        _logger.LogInformation("{SubscriberName} started.", subscriberName);

        if (!_persistentConnection.IsConnected)
        {
            await _persistentConnection.TryConnectAsync();
        }

        _channel = await _persistentConnection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: AppConstants.RabbitMq.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        await _channel.QueueDeclareAsync(
            queue: Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        foreach (var routingKey in RoutingKeys)
        {
            await _channel.QueueBindAsync(
                queue: Queue,
                exchange: AppConstants.RabbitMq.ExchangeName,
                routingKey: routingKey
            );
            _logger.LogInformation("Bound queue '{Queue}' to routing key '{RoutingKey}'", Queue, routingKey);
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            _logger.LogInformation("{SubscriberName} received message with routing key '{RoutingKey}'", subscriberName, routingKey);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetService<IMessageBrokerClientApplicationService>();


                if (service != null)
                {
                    var success = await service.ProcessMessage(routingKey, message);
                    if (success)
                    {
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                        _logger.LogInformation("{SubscriberName} successfully processed and ACKed message '{RoutingKey}'", subscriberName, routingKey);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                        _logger.LogWarning("{SubscriberName} failed to process message '{RoutingKey}', requeued.", subscriberName, routingKey);
                    }
                }
                else
                {
                    _logger.LogWarning("No service implementing IMessageBrokerClientApplicationService registered in container.");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{SubscriberName} error processing message '{RoutingKey}'", subscriberName, routingKey);
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: Queue,
            autoAck: false,
            consumer: consumer
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// Para quando o subscriber está parando.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}
