using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Application.Interfaces;

public interface ITransactionApplicationService
{
    Task<TransactionResponse> RegisterTransactionAsync(TransactionCreate request);
    Task<IEnumerable<TransactionResponse>> GetTransactionsByUserIdAsync(Guid userId);
    Task<IEnumerable<TransactionResponse>> GetTransactionsByBankAccountIdAsync(Guid bankAccountId);
    Task<PagedResult<TransactionResponse>> GetFilteredTransactionsAsync(TransactionFilterRequest request);
}
