namespace Wallet.API.Application.DTOs;

public class WalletResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public DateTime UpdatedAt { get; set; }
}
