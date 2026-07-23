using FinanceFlow.Contracts.Events;
using Wallet.API.Application.DTOs;

namespace Wallet.API.Application.Interfaces;

public interface IWalletApplicationService
{
    Task CreateWalletAsync(Guid userId);
    Task<BankAccountResponse> CreateBankAccountAsync(BankAccountCreate request);
    Task<WalletResponse> GetWalletByUserIdAsync(Guid userId);
    Task<IEnumerable<BankAccountResponse>> GetBankAccountsByUserIdAsync(Guid userId);
    Task ProcessTransactionEventAsync(TransactionCreatedIntegrationEvent transactionCreatedIntegrationEvent);
}
