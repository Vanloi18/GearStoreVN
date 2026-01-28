using GearStore.Domain.Common;
using GearStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for GearStore application
/// Manages database connections and entity configurations
/// </summary>
public class GearStoreDbContext : IdentityDbContext
{
    public GearStoreDbContext(DbContextOptions<GearStoreDbContext> options) : base(options)
    {
    }

    // DbSets for aggregate roots
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();

    // DbSets for child entities (for querying purposes)
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductSpec> ProductSpecs => Set<ProductSpec>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    
    // Additional entities
    public DbSet<SavedConfiguration> SavedConfigurations => Set<SavedConfiguration>();
    public DbSet<ConfigurationItem> ConfigurationItems => Set<ConfigurationItem>();
    public DbSet<ChatbotResponse> ChatbotResponses => Set<ChatbotResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GearStoreDbContext).Assembly);
    }
}