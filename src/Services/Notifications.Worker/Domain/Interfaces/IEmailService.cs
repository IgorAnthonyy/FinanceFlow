using System.Threading.Tasks;

namespace Notifications.Worker.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}
