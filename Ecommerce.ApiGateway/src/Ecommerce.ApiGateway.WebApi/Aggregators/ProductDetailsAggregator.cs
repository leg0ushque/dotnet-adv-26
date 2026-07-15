using Ecommerce.ApiGateway.WebApi.Models;
using MMLib.SwaggerForOcelot.Aggregates;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Ecommerce.ApiGateway.WebApi.Aggregators
{
    [AggregateResponse("Returns aggregated product details and properties", typeof(ProductFullResponse))]
    public class ProductDetailsAggregator : IDefinedAggregator
    {
        private const string ProductDetailsKey = "ProductDetails";
        private const string ProductPropertiesKey = "ProductProperties";

        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            // Resolve by route Key — safe, explicit, scales to any number of streams
            var detailsCtx = responses.FirstOrDefault(r =>
                r.Items.DownstreamRoute().Key == ProductDetailsKey);

            var propertiesCtx = responses.FirstOrDefault(r =>
                r.Items.DownstreamRoute().Key == ProductPropertiesKey);

            if (detailsCtx is null || propertiesCtx is null)
                return BuildError(
                    $"Missing downstream response. " +
                    $"Details: {detailsCtx is not null}, " +
                    $"Properties: {propertiesCtx is not null}",
                    HttpStatusCode.BadGateway);

            if (detailsCtx is null || propertiesCtx is null)
                return BuildError("Missing one or more downstream responses.", HttpStatusCode.BadGateway);

            var detailsJson = await detailsCtx.Items.DownstreamResponse().Content.ReadAsStringAsync();
            var propertiesJson = await propertiesCtx.Items.DownstreamResponse().Content.ReadAsStringAsync();

            var detailsDoc = JsonConvert.DeserializeObject<ProductResponse>(detailsJson);
            var propertiesDoc = JsonConvert.DeserializeObject<Dictionary<string, string>?>(propertiesJson);

            var result = new ProductFullResponse
            {
                Product = detailsDoc,
                Properties = propertiesDoc
            };

            var body = new StringContent(
                JsonConvert.SerializeObject(result),
                Encoding.UTF8,
                "application/json");

            return new DownstreamResponse(body, HttpStatusCode.OK, new List<Header>(), "OK");
        }

        private static DownstreamResponse BuildError(string message, HttpStatusCode code)
        {
            var body = new StringContent(
                JsonConvert.SerializeObject(new { error = message }),
                Encoding.UTF8,
                "application/json");

            return new DownstreamResponse(body, code, new List<Header>(), "Error");
        }
    }
}
