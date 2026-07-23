using System.Collections.Generic;

namespace Transactions.API.Application.DTOs;

public class DashboardReportResponse
{
    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpense { get; set; }
    public decimal CurrentMonthSavings { get; set; }
    public List<CategoryExpenseSummary> ExpensesByCategory { get; set; } = new();
    public List<MonthlyEvolutionSummary> MonthlyEvolution { get; set; } = new();
    public List<TransactionResponse> LatestTransactions { get; set; } = new();
}
