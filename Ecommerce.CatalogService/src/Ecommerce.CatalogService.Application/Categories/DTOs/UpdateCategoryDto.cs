namespace Ecommerce.CatalogService.Application.Categories.DTOs
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ParentCategoryId { get; set; }
    }
}
