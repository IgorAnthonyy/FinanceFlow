using Transactions.API.Domain.Enums;

namespace Transactions.API.Application.DTOs;

public class BankAccountTransaction
{
    public Guid BankAccountId { get; set; }
    public decimal Balance { get; set; }
}
