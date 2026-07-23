using Transactions.API.Domain.Enums;

namespace Transactions.API.Application.DTOs;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BankAccountId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
