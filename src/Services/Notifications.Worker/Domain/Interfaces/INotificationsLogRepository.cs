using System.Threading.Tasks;
using Notifications.Worker.Domain.Entities;

namespace Notifications.Worker.Domain.Interfaces;

public interface INotificationsLogRepository
{
    Task AddAsync(NotificationsLog log);
    Task SaveChangesAsync();
}
