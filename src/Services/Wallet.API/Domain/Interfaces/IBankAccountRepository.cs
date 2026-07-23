using FinanceFlow.SharedKernel.Repositories;
using Wallet.API.Domain.Entities;

namespace Wallet.API.Domain.Interfaces;

public interface IBankAccountRepository : IBaseRepository<BankAccount>
{
    Task<IEnumerable<BankAccount>> GetByUserIdAsync(Guid userId);
}
