namespace Transactions.API.Application.DTOs;

public class MonthlySummaryDto
{
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Savings => Income - Expense;
}
