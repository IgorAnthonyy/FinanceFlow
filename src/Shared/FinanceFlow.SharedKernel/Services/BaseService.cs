using FinanceFlow.SharedKernel.Entities;

namespace FinanceFlow.SharedKernel.Services;

public abstract class BaseService
{
    protected readonly UserData UserData;

    protected BaseService(UserData userData)
    {
        UserData = userData;
    }
}
