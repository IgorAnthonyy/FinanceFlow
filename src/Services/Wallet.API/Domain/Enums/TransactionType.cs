using System.ComponentModel;

namespace Wallet.API.Domain.Enums;

public enum TransactionType
{
    [Description("Entrada")]
    Income,

    [Description("Saída")]
    Expense
}
