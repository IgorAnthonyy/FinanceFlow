using Wallet.API.Domain.Enums;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Domain.Interfaces;

public interface IWalletService
{
    void RecalculateWallet(DO.Wallet wallet, IEnumerable<DO.BankAccount> accounts);
    void ProcessTransaction(DO.Wallet wallet, DO.BankAccount account, decimal amount, TransactionType transactionType);
}

