namespace Ecommerce.ApiGateway.WebApi.Models.GatewayModels;

public class AggregateErrorInfo
{
    public bool HasErrors { get; set; }
    public List<DownstreamErrorItem> Items { get; set; } = [];
}
