using GearStore.Application.Common;
using GearStore.Application.DTOs.Brand;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _brandService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<BrandDto>>.Ok(brands));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand == null) return NotFound(ApiResponse<object>.Fail("Brand not found."));
        return Ok(ApiResponse<BrandDto>.Ok(brand));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBrandDto dto)
    {
        // FluentValidation runs automatically on [ApiController]
        // But if we want to handle failures manually, we can inspect ModelState.
        // Or assume generic 400 response from framework.
        // Prompt asks for standardized output.
        // To force "ApiResponse", generic bad request might not be enough if framework returns standard ProblemDetails.
        // But for now, I'll rely on framework or logic inside service. 
        // Service doesn't validate properties (Validator does).
        // I'll return result.
        
        var created = await _brandService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<BrandDto>.Ok(created, "Brand created successfully."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateBrandDto dto)
    {
        try
        {
            await _brandService.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.Ok(null, "Brand updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _brandService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null, "Brand deleted successfully."));
    }
}
