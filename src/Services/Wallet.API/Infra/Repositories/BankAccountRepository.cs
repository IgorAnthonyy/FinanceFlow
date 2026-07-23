using Microsoft.EntityFrameworkCore;
using Wallet.API.Domain.Entities;
using Wallet.API.Domain.Interfaces;
using Wallet.API.Infra.Data;

namespace Wallet.API.Infra.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly WalletDbContext _context;

    public BankAccountRepository(WalletDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccount> GetByIdAsync(Guid id)
    {
        return await BaseQuery(tracking: true)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<BankAccount>> GetByUserIdAsync(Guid userId)
    {
        return await BaseQuery(tracking: true)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(BankAccount entity)
    {
        await _context.BankAccounts.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<BankAccount> BaseQuery(bool tracking = false)
    {
        return tracking ? _context.BankAccounts : _context.BankAccounts.AsNoTracking();
    }
}
