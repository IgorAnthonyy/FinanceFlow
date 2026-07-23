using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Extensions;

namespace Wallet.API.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    public Wallet(Guid userId)
    {
        UserId = userId;
        TotalBalance = 0;
        TotalIncome = 0;
        TotalExpense = 0;
        UpdatedAt = DateTime.UtcNow.ToBrasiliaTime();

        PrepareInsert();
    }
}
