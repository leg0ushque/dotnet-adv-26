using Ecommerce.ApiGateway.WebApi.Models.Dtos;
using Ecommerce.ApiGateway.WebApi.Models.GatewayModels;

namespace Ecommerce.ApiGateway.WebApi.Models;

public class ProductDetailsAggregationResponse : AggregationResponseBase
{
    public ProductDto? Product { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}
