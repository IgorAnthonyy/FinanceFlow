namespace FinanceFlow.SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre uma requisição não autorizada.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Cria uma nova instância de UnauthorizedException.
    /// </summary>
    /// <param name="message">A mensagem de erro.</param>
    public UnauthorizedException(string message) : base(message) { }
}