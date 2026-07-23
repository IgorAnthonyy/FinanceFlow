namespace FinanceFlow.SharedKernel.Entities;

/// <summary>
/// Classe responsável por armazenar dados do usuário.
/// </summary>
public class UserData
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
}