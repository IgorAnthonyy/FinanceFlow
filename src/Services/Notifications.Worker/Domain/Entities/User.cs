using System;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Extensions;

namespace Notifications.Worker.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }

    public User()
    {
    }

    public User(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow.ToBrasiliaTime();
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
