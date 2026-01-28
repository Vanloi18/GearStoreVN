namespace GearStore.Application.DTOs.Product;

/// <summary>
/// Request DTO for creating a new product
/// </summary>
public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
}

/// <summary>
/// Request DTO for updating an existing product
/// </summary>
public class UpdateProductRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
}

/// <summary>
/// Request DTO for updating product stock
/// </summary>
public class UpdateProductStockRequest
{
    public int ProductId { get; set; }
    public int NewStock { get; set; }
}

/// <summary>
/// Request DTO for updating product pricing
/// </summary>
public class UpdateProductPricingRequest
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
}
