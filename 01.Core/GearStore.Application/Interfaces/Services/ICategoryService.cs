using GearStore.Application.DTOs.Category;

namespace GearStore.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto);
    Task UpdateAsync(int id, CreateCategoryDto categoryDto);
    Task DeleteAsync(int id);
}
