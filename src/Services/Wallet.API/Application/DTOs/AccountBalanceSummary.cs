using System;

namespace Wallet.API.Application.DTOs;

public class AccountBalanceSummary
{
    public Guid BankAccountId { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }
    public double Percentage { get; set; }
}
