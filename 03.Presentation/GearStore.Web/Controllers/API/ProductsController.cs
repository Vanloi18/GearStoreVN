using GearStore.Application.Common;
using GearStore.Application.DTOs.Product;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterParams filterParams)
    {
        var result = await _productService.GetAllAsync(filterParams);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound(ApiResponse<object>.Fail("Product not found"));
        return Ok(ApiResponse<ProductDto>.Ok(product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductDto createDto)
    {
        try
        {
            var created = await _productService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ProductDto>.Ok(created, "Product created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateProductDto updateDto)
    {
        try
        {
            await _productService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.Ok(null, "Product updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null, "Product deleted successfully"));
    }

    // Variant Management Endpoints

    [HttpPost("/api/admin/products/{productId}/variants")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddVariant(int productId, CreateProductVariantDto dto)
    {
        try
        {
            var created = await _productService.AddVariantAsync(productId, dto);
            return Ok(ApiResponse<ProductVariantDto>.Ok(created, "Variant created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("/api/admin/products/{productId}/variants/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVariant(int productId, int id, UpdateProductVariantDto dto)
    {
        try
        {
            // Note: productId is in route but might not be needed by service if variantId is unique.
            // However, good for validation ensuring variant belongs to product.
            // For now, Service only accepts variantId, which is sufficient.
            await _productService.UpdateVariantAsync(id, dto);
            return Ok(ApiResponse<object>.Ok(null, "Variant updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("/api/admin/products/{productId}/variants/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVariant(int productId, int id)
    {
        try
        {
            await _productService.DeleteVariantAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Variant deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
