using Microsoft.EntityFrameworkCore;
using Wallet.API.Domain.Interfaces;
using Wallet.API.Infra.Data;
using DO = Wallet.API.Domain.Entities;

namespace Wallet.API.Infra.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _context;

    public WalletRepository(WalletDbContext context)
    {
        _context = context;
    }

    public async Task<DO.Wallet> GetByIdAsync(Guid id)
    {
        return await BaseQuery(tracking: true)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<DO.Wallet> GetByUserIdAsync(Guid userId)
    {
        return await BaseQuery(tracking: true)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task AddAsync(DO.Wallet entity)
    {
        await _context.Wallets.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<DO.Wallet> BaseQuery(bool tracking = false)
    {
        return tracking ? _context.Wallets : _context.Wallets.AsNoTracking();
    }
}
