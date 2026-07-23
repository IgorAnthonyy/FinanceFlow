namespace Wallet.API.Application.DTOs;

public class BankAccountCreate
{
    public Guid UserId { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }
}
