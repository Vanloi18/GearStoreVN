using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;
// =====================================================
// CONFIGURATION ITEM
// =====================================================
public class ConfigurationItemConfiguration : IEntityTypeConfiguration<ConfigurationItem>
{
    public void Configure(EntityTypeBuilder<ConfigurationItem> builder)
    {
        builder.ToTable("ConfigurationItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(i => i.ConfigurationId);

        // Relationships
        builder.HasOne(i => i.Configuration)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.ConfigurationItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}