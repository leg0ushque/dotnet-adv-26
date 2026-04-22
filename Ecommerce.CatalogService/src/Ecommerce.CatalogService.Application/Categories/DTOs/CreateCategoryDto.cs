namespace Ecommerce.CatalogService.Application.Categories.DTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ParentCategoryId { get; set; }
    }
}
