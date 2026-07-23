using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceFlow.SharedKernel.Repositories;
using Transactions.API.Application.DTOs;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Filters;

namespace Transactions.API.Domain.Interfaces;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetByBankAccountIdAsync(Guid bankAccountId);
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetFilteredTransactionsAsync(TransactionFilter filter);
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, PeriodFilterDto filter = null);
    Task<List<CategoryExpenseSummary>> GetExpensesByCategoryAsync(Guid userId, PeriodFilterDto filter = null);
    Task<List<MonthlyEvolutionSummary>> GetMonthlyEvolutionAsync(Guid userId, PeriodFilterDto filter = null);
    Task<List<MonthlyEvolutionSummary>> GetDailyEvolutionAsync(Guid userId, PeriodFilterDto filter = null);
    Task<List<TransactionResponse>> GetLatestTransactionsAsync(Guid userId, int count, PeriodFilterDto filter = null);
}
