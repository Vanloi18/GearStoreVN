using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Order aggregate root
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table name
        builder.ToTable("Orders");

        // Primary key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.UserId)
            .HasMaxLength(450); // ASP.NET Identity user ID length

        builder.HasIndex(o => o.UserId);

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

        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(o => o.Discount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

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

        // Indexes
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);

        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.OrderItems)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(o => o.StatusHistories)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.StatusHistories)
            .HasField("_statusHistories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Audit fields
        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);
    }
}
