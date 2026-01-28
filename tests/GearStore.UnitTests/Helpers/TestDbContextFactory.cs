using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.UnitTests.Helpers;

/// <summary>
/// Factory for creating in-memory test database contexts.
/// Using InMemory provider for fast unit tests (no Docker required).
/// For integration tests with real DB behavior, consider TestContainers.
/// </summary>
public static class TestDbContextFactory
{
    public static GearStoreDbContext CreateInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<GearStoreDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new GearStoreDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static GearStoreDbContext CreateInMemoryContextWithData(string databaseName)
    {
        var context = CreateInMemoryContext(databaseName);
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(GearStoreDbContext context)
    {
        // Seed minimal test data
        var category = new GearStore.Domain.Entities.Category
        {
            Id = 1,
            Name = "Test Category",
            Slug = "test-category",
            IsActive = true
        };

        var brand = new GearStore.Domain.Entities.Brand
        {
            Id = 1,
            Name = "Test Brand",
            Slug = "test-brand",
            IsActive = true
        };

        context.Categories.Add(category);
        context.Brands.Add(brand);
        context.SaveChanges();
    }
}
