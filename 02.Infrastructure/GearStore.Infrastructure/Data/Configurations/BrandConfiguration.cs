using GearStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Brand entity
/// </summary>
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        // Table name
        builder.ToTable("Brands");

        // Primary key
        builder.HasKey(b => b.Id);

        // Properties
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(b => b.Slug)
            .IsUnique();

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.LogoUrl)
            .HasMaxLength(500);

        builder.Property(b => b.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(b => b.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.IsFeatured)
            .HasDefaultValue(false);

        // Relationships - Products collection
        builder.HasMany(b => b.Products)
            .WithOne(p => p.Brand)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt);
    }
}
