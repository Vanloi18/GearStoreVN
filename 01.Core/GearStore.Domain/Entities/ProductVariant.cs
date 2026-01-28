using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

public class ProductVariant : AuditableEntity
{
    private ProductVariant() { }

    public ProductVariant(int productId, string name, decimal price, int stock, string? sku = null)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Stock = stock;
        SKU = sku;
    }

    public int ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string? SKU { get; private set; }

    public Product Product { get; private set; } = null!;

    public void Update(string name, decimal price, int stock, string? sku)
    {
        Name = name;
        Price = price;
        Stock = stock;
        SKU = sku;
        MarkAsUpdated();
    }

    public void DecreaseStock(int quantity)
    {
        if (Stock < quantity) throw new InvalidOperationException($"Insufficient stock for variant {Name}");
        Stock -= quantity;
        MarkAsUpdated();
    }

    public void IncreaseStock(int quantity)
    {
        Stock += quantity;
        MarkAsUpdated();
    }
}
