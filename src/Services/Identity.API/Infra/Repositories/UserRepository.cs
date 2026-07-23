using Identity.API.Domain.Entities;
using Identity.API.Domain.Interfaces;
using Identity.API.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await BaseQuery(true)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<User> BaseQuery(bool tracking = false)
    {
        return tracking ? _context.Users : _context.Users.AsNoTracking();
    }
}