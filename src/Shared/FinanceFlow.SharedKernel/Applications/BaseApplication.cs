using FinanceFlow.SharedKernel.Entities;

namespace FinanceFlow.SharedKernel.Applications;

/// <summary>
/// Classe base para aplicações.
/// </summary>
public abstract class BaseApplication
{
    protected readonly UserData UserData;

    /// <summary>
    /// Construtor da classe BaseApplication.
    /// </summary>
    /// <param name="userData">Dados do usuário.</param>
    protected BaseApplication(UserData userData)
    {
        UserData = userData;
    }
}
