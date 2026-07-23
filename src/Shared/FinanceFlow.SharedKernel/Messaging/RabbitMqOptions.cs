namespace FinanceFlow.SharedKernel.Messaging;

/// <summary>
/// Classe responsável por armazenar as configurações do RabbitMQ.
/// </summary>
public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "financeflow-exchange";
    public int RetryCount { get; set; } = 15;
}
