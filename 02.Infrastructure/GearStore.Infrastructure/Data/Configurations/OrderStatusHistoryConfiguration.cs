using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;
// =====================================================
// ORDER STATUS HISTORY CONFIGURATION
// =====================================================
public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.OldStatus)
            .HasMaxLength(50);

        builder.Property(h => h.NewStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.Notes)
            .HasMaxLength(500);

        builder.Property(h => h.ChangedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(h => h.Order)
            .WithMany(o => o.StatusHistories)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}