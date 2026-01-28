using FluentValidation;
using GearStore.Application.DTOs.Brand;
using GearStore.Application.DTOs.Category;
using GearStore.Application.DTOs.Product;

namespace GearStore.Application.Validators;

public class CreateBrandDtoValidator : AbstractValidator<CreateBrandDto>
{
    public CreateBrandDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Brand name is required.");
        RuleFor(x => x.Logo).NotEmpty().WithMessage("Logo URL is required.");
    }
}

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
    }
}

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category ID is required.");
        RuleFor(x => x.BrandId).GreaterThan(0).WithMessage("Brand ID is required.");
    }
}
