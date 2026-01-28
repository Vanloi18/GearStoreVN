namespace GearStore.Application.DTOs.Product;

public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
