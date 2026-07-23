using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Worker.Domain.Entities;

namespace Notifications.Worker.Infra.Data.Configurations;

public class NotificationsLogConfiguration : IEntityTypeConfiguration<NotificationsLog>
{
    public void Configure(EntityTypeBuilder<NotificationsLog> builder)
    {
        builder.ToTable("NotificationsLogs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.EventName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.RecipientEmail).IsRequired().HasMaxLength(150);
    }
}
