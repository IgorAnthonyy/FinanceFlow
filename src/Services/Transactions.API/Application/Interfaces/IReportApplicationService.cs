using System.Collections.Generic;
using System.Threading.Tasks;
using Transactions.API.Application.DTOs;

namespace Transactions.API.Application.Interfaces;

public interface IReportApplicationService
{
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(PeriodFilterDto filter = null);
    Task<List<CategoryExpenseSummary>> GetExpensesByCategoryAsync(PeriodFilterDto filter = null);
    Task<List<MonthlyEvolutionSummary>> GetMonthlyEvolutionAsync(PeriodFilterDto filter = null);
    Task<List<MonthlyEvolutionSummary>> GetDailyEvolutionAsync(PeriodFilterDto filter = null);
    Task<List<TransactionResponse>> GetLatestTransactionsAsync(PeriodFilterDto filter = null);
}
