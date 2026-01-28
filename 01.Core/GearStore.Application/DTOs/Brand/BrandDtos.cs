namespace GearStore.Application.DTOs.Brand;

public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}

public class CreateBrandDto
{
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}
