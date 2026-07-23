namespace FinanceFlow.Contracts.Events;

/// <summary>
/// Evento de usuário criado.
/// </summary>
public record UserCreatedIntegrationEvent(
    Guid Id,

    string Name,

    string Email,
    DateTime CreatedAt);
