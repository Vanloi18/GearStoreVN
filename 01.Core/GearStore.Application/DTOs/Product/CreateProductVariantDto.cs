using System.ComponentModel.DataAnnotations;

namespace GearStore.Application.DTOs.Product;

public class CreateProductVariantDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public string? SKU { get; set; }
}
