namespace Ecommerce.CatalogService.Application.Common.DTOs.QueueMessages
{
    public class UpdatedProductMessage
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
    }
}
