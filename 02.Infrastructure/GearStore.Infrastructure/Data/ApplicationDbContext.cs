using GearStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Data;

/// <summary>
/// DbContext chính của ứng dụng
/// Kế thừa IdentityDbContext để có sẵn bảng User, Role
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets - Đại diện cho các bảng trong database
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSpec> ProductSpecs { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<SavedConfiguration> SavedConfigurations { get; set; }
    public DbSet<ConfigurationItem> ConfigurationItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<ChatbotResponse> ChatbotResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply tất cả configurations từ assembly hiện tại
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Override SaveChanges để tự động set UpdatedAt
        // (Sẽ implement ở phần sau)
    }

    /// <summary>
    /// Override SaveChanges để tự động cập nhật UpdatedAt
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditableEntities();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync để tự động cập nhật UpdatedAt
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Tự động set CreatedAt và UpdatedAt cho các entity kế thừa AuditableEntity
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.AuditableEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}