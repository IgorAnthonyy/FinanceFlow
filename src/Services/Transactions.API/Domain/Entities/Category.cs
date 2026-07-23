using FinanceFlow.SharedKernel.Entities;

namespace Transactions.API.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }
    public bool IsDefault { get; set; }
    public Guid? UserId { get; set; }

    public Category() { }

    public Category(string name, string icon, string color, bool isDefault = true)
    {
        Name = name;
        Icon = icon;
        Color = color;
        IsDefault = isDefault;

        base.PrepareInsert();
    }

    public void PrepareInsert(UserData userData)
    {
        base.PrepareInsert();

        UserId = userData.Id;
        IsDefault = false;
    }
}
