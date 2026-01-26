using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;
// =====================================================
// CHATBOT RESPONSE CONFIGURATION
// =====================================================
public class ChatbotResponseConfiguration : IEntityTypeConfiguration<ChatbotResponse>
{
    public void Configure(EntityTypeBuilder<ChatbotResponse> builder)
    {
        builder.ToTable("ChatbotResponses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Keywords)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.ResponseText)
            .IsRequired();

        builder.Property(c => c.Category)
            .HasMaxLength(100);

        builder.Property(c => c.Priority)
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}