using System;
using FinanceFlow.SharedKernel.Entities;

namespace Notifications.Worker.Domain.Entities;

public class NotificationsLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string EventName { get; set; }
    public string RecipientEmail { get; set; }

    public NotificationsLog()
    {
    }

    public NotificationsLog(Guid userId, string eventName, string recipientEmail)
    {
        UserId = userId;
        EventName = eventName;
        RecipientEmail = recipientEmail;
        PrepareInsert();
    }
}
