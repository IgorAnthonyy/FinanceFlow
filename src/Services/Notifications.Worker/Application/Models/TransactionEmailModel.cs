using System;

namespace Notifications.Worker.Application.Models;

public class TransactionEmailModel
{
    public decimal Amount { get; }
    public string Type { get; }
    public Guid BankAccountId { get; }
    public DateTime CreatedAt { get; }

    public string FormattedAmount => $"R$ {Amount:N2}";
    public string BankAccountName => $"Ref. Conta: {BankAccountId.ToString().Substring(0, 8)}...";
    public string Date => CreatedAt.ToString("g");

    public TransactionEmailModel(decimal amount, string type, Guid bankAccountId, DateTime createdAt)
    {
        Amount = amount;
        Type = type;
        BankAccountId = bankAccountId;
        CreatedAt = createdAt;
    }
}
