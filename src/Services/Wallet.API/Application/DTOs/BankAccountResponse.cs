namespace Wallet.API.Application.DTOs;

public class BankAccountResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}
