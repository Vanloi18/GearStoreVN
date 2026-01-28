using GearStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GearStore.Infrastructure.Data.Configurations;

public class ChatbotResponseConfiguration : IEntityTypeConfiguration<ChatbotResponse>
{
    public void Configure(EntityTypeBuilder<ChatbotResponse> builder)
    {
        builder.ToTable("ChatbotResponses");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Keywords).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Answer).IsRequired();
        builder.Property(c => c.MatchType).HasMaxLength(50);
        builder.Property(c => c.Priority).HasDefaultValue(0);
    }
}