using RabbitMQ.Client;

namespace FinanceFlow.SharedKernel.Messaging.Abstractions;

/// <summary>
/// Interface para conexão persistente com o RabbitMQ.
/// </summary>
public interface IRabbitMqPersistentConnection : IDisposable
{
    /// <summary>
    /// Verifica se a conexão está conectada.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Tenta conectar ao RabbitMQ.
    /// </summary>
    Task<bool> TryConnectAsync();

    /// <summary>
    /// Cria um canal.
    /// </summary>
    Task<IChannel> CreateChannelAsync();
}
