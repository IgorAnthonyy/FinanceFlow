namespace Transactions.API.Application.DTOs;

public class CategoryExpenseSummary
{
    public string CategoryName { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
}
