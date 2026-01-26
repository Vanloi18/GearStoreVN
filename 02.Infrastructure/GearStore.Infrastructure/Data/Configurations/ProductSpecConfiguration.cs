using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

// =====================================================
// PRODUCT SPEC CONFIGURATION
// =====================================================
public class ProductSpecConfiguration : IEntityTypeConfiguration<ProductSpec>
{
    public void Configure(EntityTypeBuilder<ProductSpec> builder)
    {
        builder.ToTable("ProductSpecs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SpecKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.SpecValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.DisplayName)
            .HasMaxLength(100);

        // Index quan trọng cho PC Builder - tìm kiếm theo SpecKey và SpecValue
        builder.HasIndex(s => new { s.SpecKey, s.SpecValue });
        builder.HasIndex(s => s.ProductId);

        // Relationships
        builder.HasOne(s => s.Product)
            .WithMany(p => p.Specs)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}