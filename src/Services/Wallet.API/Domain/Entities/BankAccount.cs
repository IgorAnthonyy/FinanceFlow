using FinanceFlow.SharedKernel.Entities;

namespace Wallet.API.Domain.Entities;

public class BankAccount : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }

    public virtual Wallet Wallet { get; set; }

    protected BankAccount() { }
}
