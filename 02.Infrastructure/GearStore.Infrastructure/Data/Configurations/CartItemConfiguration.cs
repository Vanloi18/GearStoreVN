using GearStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for CartItem entity
/// </summary>
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        // Table name
        builder.ToTable("CartItems");

        // Primary key
        builder.HasKey(ci => ci.Id);

        // Properties
        builder.Property(ci => ci.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ci => ci.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        // Relationships
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for product lookup
        builder.HasIndex(ci => ci.ProductId);
    }
}
