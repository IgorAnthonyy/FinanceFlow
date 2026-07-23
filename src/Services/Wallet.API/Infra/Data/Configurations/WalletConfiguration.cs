using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletEntity = Wallet.API.Domain.Entities.Wallet;

namespace Wallet.API.Infra.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.TotalBalance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(w => w.TotalIncome)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(w => w.TotalExpense)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(w => w.UpdatedAt)
            .IsRequired();
    }
}
