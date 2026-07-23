using FinanceFlow.SharedKernel.Repositories;
using Wallet.API.Domain.Entities;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Domain.Interfaces;

public interface IWalletRepository : IBaseRepository<DO.Wallet>
{
    Task<DO.Wallet> GetByUserIdAsync(Guid userId);
}
