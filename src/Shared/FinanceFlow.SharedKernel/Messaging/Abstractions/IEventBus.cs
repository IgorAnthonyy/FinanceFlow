namespace FinanceFlow.SharedKernel.Messaging.Abstractions;

/// <summary>
/// Interface para publicar eventos.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publica um evento.
    /// </summary>
    /// <param name="@event">O evento a ser publicado.</param>
    /// <param name="routingKey">A chave de roteamento.</param>
    /// <param name="exchange">O exchange.</param>
    Task PublishAsync<T>(T @event, string routingKey, string exchange) where T : class;
}
