namespace FinanceFlow.SharedKernel.Messaging.Abstractions;

/// <summary>
/// Interface para processar mensagens do message broker.
/// </summary>
public interface IMessageBrokerClientApplicationService : IDisposable
{
    /// <summary>
    /// Processa uma mensagem.
    /// </summary>
    /// <param name="routingKey">Chave de roteamento.</param>
    /// <param name="message">Mensagem a ser processada.</param>
    /// <returns>True se a mensagem foi processada com sucesso, false caso contrário.</returns>
    Task<bool> ProcessMessage(string routingKey, string message);
}
