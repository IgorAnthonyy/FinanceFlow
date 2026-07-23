using FinanceFlow.SharedKernel.Messaging;
using FinanceFlow.SharedKernel.Messaging.Abstractions;

namespace Notifications.Worker.Consumers;

public class NotificationQueueConsumer : BaseRabbitMqSubscriber
{
    protected override string Queue => "notification-service-queue";

    protected override IEnumerable<string> RoutingKeys =>
    [
        AppConstants.RabbitMq.RoutingKeys.UserCreated,
        AppConstants.RabbitMq.RoutingKeys.TransactionCreated,
        AppConstants.RabbitMq.RoutingKeys.UserUpdated
    ];

    public NotificationQueueConsumer(
        IRabbitMqPersistentConnection persistentConnection,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationQueueConsumer> logger)
        : base(persistentConnection, scopeFactory, logger)
    {
    }
}
