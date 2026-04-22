using Ecommerce.CatalogService.Domain.Common;

namespace Ecommerce.CatalogService.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? ParentCategoryId { get; set; }

        public Category()
        { }

        public Category(string id, string name, string? imageUrl = null, string? parentCategoryId = null)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            ParentCategoryId = parentCategoryId;
        }

        public void UpdateDetails(string name, string? imageUrl, string? parentCategoryId)
        {
            Name = name;
            ImageUrl = imageUrl;
            ParentCategoryId = parentCategoryId;
        }
    }
}
