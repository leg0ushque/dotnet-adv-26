using Ecommerce.CatalogService.Domain.Common;

namespace Ecommerce.CatalogService.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string CategoryId { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Amount { get; set; }

        public Product()
        { }

        public Product(string id, string name, string categoryId, decimal price, int amount, 
            string? description = null, string? imageUrl = null)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            Price = price;
            Amount = amount;
            Description = description;
            ImageUrl = imageUrl;
        }

        public void UpdateDetails(string name, string categoryId, decimal price, int amount, 
            string? description, string? imageUrl)
        {
            Name = name;
            CategoryId = categoryId;
            Price = price;
            Amount = amount;
            Description = description;
            ImageUrl = imageUrl;
        }
    }
}
