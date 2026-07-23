using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.API.Domain.Entities;

namespace Transactions.API.Infra.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Icon)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.IsDefault)
            .IsRequired();


        builder.Property(c => c.UserId);

        var defaultCategories = GenerateDefaultCategories();

        builder.HasData(defaultCategories);
    }

    private static List<Category> GenerateDefaultCategories()
    {
        var categories = new List<Category>()
        {
            new Category("Alimentação", "utensils", "orange", true)
            {
                Id = Guid.Parse("d3b07384-d113-4ae3-a5c2-f1556093d56d"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            new Category("Salário", "briefcase", "green", true)
            {
                Id = Guid.Parse("2df511ff-12a1-432d-83cb-980ec8dcbe80"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            new Category("Transporte", "car", "blue", true)
            {
                Id = Guid.Parse("a4a5b422-b5e1-4554-b529-8735df145b34"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            new Category("Lazer", "gamepad", "purple", true)
            {
                Id = Guid.Parse("f8c9b2cc-ee0e-4364-b258-29be309be0d1"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        return categories;
    }
}
