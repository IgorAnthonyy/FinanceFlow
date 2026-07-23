using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Notifications.Worker.Domain.Interfaces;
using Notifications.Worker.Infra.Data;
using Notifications.Worker.Domain.Entities;

namespace Notifications.Worker.Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NotificationDbContext _context;

    public UserRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<(string Name, string Email)> GetUserInfoAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return (null, null);

        return (user.Name, user.Email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByIdAsync(Guid userId, bool tracking = false)
    {
        if (tracking)
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
    }
}
