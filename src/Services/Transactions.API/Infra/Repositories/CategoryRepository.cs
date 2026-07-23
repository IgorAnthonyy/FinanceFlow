using Microsoft.EntityFrameworkCore;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Interfaces;
using Transactions.API.Infra.Data;

namespace Transactions.API.Infra.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly TransactionsDbContext _context;

    public CategoryRepository(TransactionsDbContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid userId)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsDefault || c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Category> GetByNameAsync(string name)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task AddAsync(Category entity)
    {
        await _context.Categories.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
