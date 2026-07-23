using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.API.Domain.Entities;

namespace Wallet.API.Infra.Data.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasOne(b => b.Wallet)
            .WithMany(w => w.BankAccounts)
            .HasForeignKey(b => b.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
