using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

// =====================================================
// PRODUCT CONFIGURATION
// =====================================================
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.SKU)
            .HasMaxLength(50);

        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.Property(p => p.Brand)
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.OriginalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.ShortDescription)
            .HasMaxLength(500);

        builder.Property(p => p.ThumbnailImage)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes for performance
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.IsActive);

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Specs)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}