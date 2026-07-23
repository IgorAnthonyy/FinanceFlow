namespace Notifications.Worker.Application.Models;

public class TransactionNotificationViewModel
{
    public UserEmailModel User { get; }
    public TransactionEmailModel Transaction { get; }

    public TransactionNotificationViewModel(UserEmailModel user, TransactionEmailModel transaction)
    {
        User = user;
        Transaction = transaction;
    }
}
