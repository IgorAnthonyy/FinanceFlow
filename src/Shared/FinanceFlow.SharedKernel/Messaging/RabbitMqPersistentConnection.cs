using FinanceFlow.SharedKernel.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinanceFlow.SharedKernel.Messaging;

/// <summary>
/// Classe responsável por gerenciar a conexão persistente com o RabbitMQ.
/// </summary>
public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
{
    /// <summary>
    /// Fábrica de conexões com o RabbitMQ.
    /// </summary>
    private readonly IConnectionFactory _connectionFactory;

    /// <summary>
    /// Logger para registrar informações sobre a conexão com o RabbitMQ.
    /// </summary>
    private readonly ILogger<RabbitMqPersistentConnection> _logger;

    /// <summary>
    /// Número de tentativas de conexão com o RabbitMQ.
    /// </summary>
    private readonly int _retryCount;

    /// <summary>
    /// Conexão com o RabbitMQ.
    /// </summary>
    private IConnection _connection;

    /// <summary>
    /// Indica se a conexão com o RabbitMQ foi encerrada.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Semáforo para controlar o acesso à conexão com o RabbitMQ.
    /// </summary>
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    /// <summary>
    /// Construtor do RabbitMqPersistentConnection.
    /// </summary>
    /// <param name="connectionFactory">Fábrica de conexões com o RabbitMQ.</param>
    /// <param name="logger">Logger para registrar informações sobre a conexão com o RabbitMQ.</param>
    /// <param name="retryCount">Número de tentativas de conexão com o RabbitMQ.</param>
    public RabbitMqPersistentConnection(
        IConnectionFactory connectionFactory,
        ILogger<RabbitMqPersistentConnection> logger,
        int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    /// <summary>
    /// Verifica se a conexão com o RabbitMQ está ativa.
    /// </summary>
    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    /// <summary>
    /// Cria um canal para comunicação com o RabbitMQ.
    /// </summary>
    /// <returns>Um canal para comunicação com o RabbitMQ.</returns>
    public async Task<IChannel> CreateChannelAsync()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Nenhuma conexão com o RabbitMQ está ativa para criar um canal.");
        }

        return await _connection.CreateChannelAsync();
    }

    /// <summary>
    /// Tenta se conectar ao RabbitMQ.
    /// </summary>
    /// <returns>True se a conexão foi estabelecida com sucesso, false caso contrário.</returns>
    public async Task<bool> TryConnectAsync()
    {
        _logger.LogInformation("Tentando se conectar ao RabbitMQ...");

        await _connectionLock.WaitAsync();
        try
        {
            if (IsConnected) return true;

            for (int i = 0; i < _retryCount; i++)
            {
                try
                {
                    _connection = await _connectionFactory.CreateConnectionAsync();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao conectar ao RabbitMQ. Tentativa {Tentativa} de {Total}", i + 1, _retryCount);
                    if (i == _retryCount - 1) throw;
                    await Task.Delay(2000);
                }
            }

            if (IsConnected)
            {
                _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso!");
                return true;
            }

            return false;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <summary>
    /// Evento chamado quando a conexão com o RabbitMQ é bloqueada.
    /// </summary>
    /// <param name="sender">O remetente.</param>
    /// <param name="e">Os argumentos do evento.</param>
    private Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning("Conexão com RabbitMQ bloqueada. Tentando reconectar...");
        return TryConnectAsync();
    }

    /// <summary>
    /// Evento chamado quando ocorre uma exceção de callback no RabbitMQ.
    /// </summary>
    /// <param name="sender">O remetente.</param>
    /// <param name="e">Os argumentos do evento.</param>
    private Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning(e.Exception, "Exceção de callback no RabbitMQ. Tentando reconectar...");
        return TryConnectAsync();
    }

    /// <summary>
    /// Evento chamado quando a conexão com o RabbitMQ é encerrada.
    /// </summary>
    /// <param name="sender">O remetente.</param>
    /// <param name="reason">Os argumentos do evento.</param>
    private Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning("Conexão com RabbitMQ encerrada. Tentando reconectar...");
        return TryConnectAsync();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex.ToString());
        }
    }
}
