namespace FinanceFlow.SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre um conflito de dados.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Cria uma nova instância de ConflictException.
    /// </summary>
    /// <param name="message">A mensagem de erro.</param>
    public ConflictException(string message) : base(message) { }
}