namespace GearStore.Application.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Added default value
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailImage { get; set; }
    
    public ICollection<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
}
