using System;
using System.Threading.Tasks;
using Notifications.Worker.Domain.Entities;

namespace Notifications.Worker.Domain.Interfaces;

public interface IUserRepository
{
    Task<(string Name, string Email)> GetUserInfoAsync(Guid userId);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User> GetByIdAsync(Guid id, bool tracking = false);
}
