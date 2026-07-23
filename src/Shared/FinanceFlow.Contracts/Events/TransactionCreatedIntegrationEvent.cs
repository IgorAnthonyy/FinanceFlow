namespace FinanceFlow.Contracts.Events;

/// <summary>
/// Evento de transação criada.
/// </summary>
public record TransactionCreatedIntegrationEvent(
    Guid UserId,
    decimal Amount,
    string Type,
    Guid BankAccountId,
    DateTime CreatedAt);
