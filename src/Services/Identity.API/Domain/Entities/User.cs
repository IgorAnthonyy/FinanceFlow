using FinanceFlow.SharedKernel.Entities;

namespace Identity.API.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;

        PrepareInsert();
    }

    public void UpdateInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
