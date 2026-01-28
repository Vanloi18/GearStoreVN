namespace GearStore.Application.DTOs.Product;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? SKU { get; set; }
}
