using System.ComponentModel;

namespace Transactions.API.Domain.Enums;

public enum TransactionType
{
    [Description("Entrada")]
    Income,

    [Description("Saída")]
    Expense
}
