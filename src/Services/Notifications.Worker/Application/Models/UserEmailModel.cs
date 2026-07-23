namespace Notifications.Worker.Application.Models;

public class UserEmailModel
{
    public string Name { get; }
    public string Email { get; }

    public UserEmailModel(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
