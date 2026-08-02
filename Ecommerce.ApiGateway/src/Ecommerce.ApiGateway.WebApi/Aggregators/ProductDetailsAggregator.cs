using System.Net;
using Ecommerce.ApiGateway.WebApi.Models;
using Ecommerce.ApiGateway.WebApi.Models.Downstream;
using Ecommerce.ApiGateway.WebApi.Models.Dtos;
using MMLib.SwaggerForOcelot.Aggregates;
using Newtonsoft.Json;
using Ocelot.Middleware;

namespace Ecommerce.ApiGateway.WebApi.Aggregators;

[AggregateResponse("Returns aggregated product details and properties", typeof(ProductDetailsAggregationResponse))]
public class ProductDetailsAggregator(ILogger<ProductDetailsAggregator> logger)
    : AggregatorBase<ProductDetailsAggregationResponse>(logger)
{
    private const string ProductDetailsKey = "ProductDetails";
    private const string ProductPropertiesKey = "ProductProperties";

    protected override string[] RequiredRouteKeys => [ProductDetailsKey, ProductPropertiesKey];

    protected override async Task<DownstreamResponse> AggregateSuccessfulResponses(
        Dictionary<string, DownstreamResponse> responses)
    {
        var detailsResponse = responses[ProductDetailsKey];
        var propertiesResponse = responses[ProductPropertiesKey];

        _logger.LogDebug("Starting aggregation of product details and properties");

        var aggregationResponse = new ProductDetailsAggregationResponse();

        var detailsJson = await detailsResponse.Content.ReadAsStringAsync();

        var detailsResource = JsonConvert.DeserializeObject<ResourceResponse<ProductDto>>(detailsJson);
        aggregationResponse.Product = detailsResource?.Data;

        var propertiesJson = await propertiesResponse.Content.ReadAsStringAsync();

        aggregationResponse.Properties = JsonConvert.DeserializeObject<Dictionary<string, string>?>(propertiesJson);

        _logger.LogDebug("Aggregation complete. Product: {ProductName}", aggregationResponse.Product?.Name ?? "null");

        return BuildSuccessResponse(aggregationResponse);
    }

    protected override HttpStatusCode MapDownstreamStatus(HttpStatusCode downstreamStatus)
    {
        return downstreamStatus switch
        {
            HttpStatusCode.NotFound => HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest => HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized => HttpStatusCode.Unauthorized,

            HttpStatusCode.InternalServerError => HttpStatusCode.BadGateway,
            _ => base.MapDownstreamStatus(downstreamStatus)
        };
    }
}
