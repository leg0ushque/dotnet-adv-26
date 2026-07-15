namespace Ecommerce.ApiGateway.WebApi.Models
{
    public class ProductFullResponse
    {
        public ProductResponse? Product { get; set; }
        public Dictionary<string, string>? Properties { get; set; }
    }

    public class ProductResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
