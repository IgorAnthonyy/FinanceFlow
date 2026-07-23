using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Extensions;
using FinanceFlow.SharedKernel.Services;
using Wallet.API.Domain.Enums;
using Wallet.API.Domain.Interfaces;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Domain.Services;

public class WalletService : BaseService, IWalletService
{
    public WalletService(UserData userData) : base(userData)
    {
    }

    public void RecalculateWallet(DO.Wallet wallet, IEnumerable<DO.BankAccount> accounts)
    {
        wallet.TotalBalance = accounts.Sum(a => a.Balance);
        wallet.UpdatedAt = DateTime.UtcNow.ToBrasiliaTime();
    }

    public void ProcessTransaction(DO.Wallet wallet, DO.BankAccount account, decimal amount, TransactionType transactionType)
    {
        if (transactionType == TransactionType.Income)
        {
            account.Balance += amount;
            wallet.TotalIncome += amount;
        }
        else if (transactionType == TransactionType.Expense)
        {
            account.Balance -= amount;
            wallet.TotalExpense += amount;
        }

        wallet.UpdatedAt = DateTime.UtcNow.ToBrasiliaTime();
    }
}
