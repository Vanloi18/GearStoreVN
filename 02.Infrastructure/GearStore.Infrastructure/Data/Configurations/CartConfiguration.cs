using GearStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Cart aggregate root
/// </summary>
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        // Table name
        builder.ToTable("Carts");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.UserId)
            .HasMaxLength(450); // ASP.NET Identity user ID length

        builder.HasIndex(c => c.UserId);

        builder.Property(c => c.SessionId)
            .HasMaxLength(200);

        builder.HasIndex(c => c.SessionId);

        // Owned collection (child entities)
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);
    }
}
