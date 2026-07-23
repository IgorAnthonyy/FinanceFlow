using Microsoft.EntityFrameworkCore;
using Wallet.API.Domain.Entities;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Infra.Data;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }
    public DbSet<DO.Wallet> Wallets { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
