namespace FinanceFlow.Contracts.Events;

/// <summary>
/// Evento de usuário atualizado.
/// </summary>
public record UserUpdatedIntegrationEvent(
    Guid Id,
    string Name,
    string Email);
