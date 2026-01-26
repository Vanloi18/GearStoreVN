using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

// =====================================================
// ORDER CONFIGURATION
// =====================================================
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.CustomerEmail)
            .HasMaxLength(200);

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.SubTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(o => o.Discount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // Enum conversion
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(PaymentStatus.Unpaid);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.AdminNotes)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);

        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.StatusHistories)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}