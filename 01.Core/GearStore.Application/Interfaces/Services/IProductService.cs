using GearStore.Application.Common;
using GearStore.Application.DTOs.Product;

namespace GearStore.Application.Interfaces.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllAsync(ProductFilterParams filterParams);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task UpdateAsync(int id, CreateProductDto updateDto);
    Task DeleteAsync(int id);
    
    // Variant Management
    Task<ProductVariantDto> AddVariantAsync(int productId, CreateProductVariantDto dto);
    Task UpdateVariantAsync(int variantId, UpdateProductVariantDto dto);
    Task DeleteVariantAsync(int variantId);
}
