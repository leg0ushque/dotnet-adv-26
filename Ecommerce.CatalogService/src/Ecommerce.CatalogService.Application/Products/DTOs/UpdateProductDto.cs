namespace Ecommerce.CatalogService.Application.Products.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}
