using System.Threading.Tasks;
using Notifications.Worker.Domain.Entities;
using Notifications.Worker.Domain.Interfaces;
using Notifications.Worker.Infra.Data;

namespace Notifications.Worker.Infra.Repositories;

public class NotificationsLogRepository : INotificationsLogRepository
{
    private readonly NotificationDbContext _context;

    public NotificationsLogRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NotificationsLog log)
    {
        await _context.NotificationsLogs.AddAsync(log);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
