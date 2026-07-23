using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Extensions;
using Transactions.API.Domain.Enums;

namespace Transactions.API.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BankAccountId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public DateTime TransactionDate { get; set; }

    public Transaction()
    {
    }

    public Transaction(
        Guid userId,
        Guid bankAccountId,
        string title,
        string description,
        decimal amount,
        TransactionType type,
        Guid categoryId)
    {
        UserId = userId;
        BankAccountId = bankAccountId;
        Title = title;
        Description = description;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
    }

    public override void PrepareInsert()
    {
        TransactionDate = DateTime.UtcNow.ToBrasiliaTime();
        base.PrepareInsert();
    }
}
