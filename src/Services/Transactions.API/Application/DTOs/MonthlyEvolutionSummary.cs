namespace Transactions.API.Application.DTOs;

public class MonthlyEvolutionSummary
{
    public string MonthName { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}
