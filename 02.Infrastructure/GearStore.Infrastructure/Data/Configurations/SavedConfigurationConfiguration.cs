using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

// =====================================================
// SAVED CONFIGURATION
// =====================================================
public class SavedConfigurationConfiguration : IEntityTypeConfiguration<SavedConfiguration>
{
    public void Configure(EntityTypeBuilder<SavedConfiguration> builder)
    {
        builder.ToTable("SavedConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConfigurationName)
            .HasMaxLength(200);

        builder.Property(c => c.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Configuration)
            .HasForeignKey(i => i.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}