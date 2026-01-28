using FluentAssertions;
using GearStore.Application.Interfaces;
using GearStore.Application.Services;
using GearStore.Domain.Entities;
using GearStore.UnitTests.Helpers;
using Moq;
using Xunit;

namespace GearStore.UnitTests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly GearStore.Infrastructure.Data.GearStoreDbContext _context;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContextWithData(Guid.NewGuid().ToString());
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _unitOfWorkMock.Setup(u => u.Products).Returns(new GearStore.Infrastructure.Repositories.ProductRepository(_context));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _productService = new ProductService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingProduct_ShouldSetIsActiveFalse()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Slug = "test-product",
            SKU = "TEST-001",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        await _productService.SoftDeleteAsync(1);

        // Assert
        var deletedProduct = await _context.Products.FindAsync(1);
        deletedProduct.Should().NotBeNull();
        deletedProduct!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetActiveProductsAsync_ShouldExcludeDeletedProducts()
    {
        // Arrange
        var activeProduct = new Product
        {
            Id = 2,
            Name = "Active Product",
            Slug = "active-product",
            SKU = "ACTIVE-001",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        var deletedProduct = new Product
        {
            Id = 3,
            Name = "Deleted Product",
            Slug = "deleted-product",
            SKU = "DELETED-001",
            Price = 50,
            Stock = 5,
            CategoryId = 1,
            BrandId = 1,
            IsActive = false
        };
        _context.Products.AddRange(activeProduct, deletedProduct);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productService.GetActiveProductsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
        result.First().Id.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_DeletedProduct_ShouldReturnNull()
    {
        // Arrange
        var product = new Product
        {
            Id = 4,
            Name = "Test Product",
            Slug = "test-product-4",
            SKU = "TEST-004",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = false
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productService.GetActiveByIdAsync(4);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStockAsync_ValidQuantity_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Id = 5,
            Name = "Stock Test Product",
            Slug = "stock-test",
            SKU = "STOCK-001",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        await _productService.UpdateStockAsync(5, 15);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(5);
        updatedProduct!.Stock.Should().Be(15);
    }

    [Fact]
    public async Task UpdateStockAsync_NegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var product = new Product
        {
            Id = 6,
            Name = "Test Product",
            Slug = "test-product-6",
            SKU = "TEST-006",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _productService.UpdateStockAsync(6, -5)
        );
    }

    [Fact]
    public async Task SearchProductsAsync_ByName_ShouldReturnMatchingProducts()
    {
        // Arrange
        var product1 = new Product
        {
            Id = 7,
            Name = "Gaming Laptop",
            Slug = "gaming-laptop",
            SKU = "LAPTOP-001",
            Price = 1000,
            Stock = 5,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        var product2 = new Product
        {
            Id = 8,
            Name = "Gaming Mouse",
            Slug = "gaming-mouse",
            SKU = "MOUSE-001",
            Price = 50,
            Stock = 20,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        var product3 = new Product
        {
            Id = 9,
            Name = "Office Keyboard",
            Slug = "office-keyboard",
            SKU = "KEYBOARD-001",
            Price = 30,
            Stock = 15,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.AddRange(product1, product2, product3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productService.SearchAsync("Gaming");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Name.Should().Contain("Gaming"));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
