namespace Ecommerce.ApiGateway.WebApi.Models.GatewayModels;

public class DownstreamErrorItem
{
    public string Key { get; set; } = default!;
    public int StatusCode { get; set; }
    public string? ReasonPhrase { get; set; }
    public string? Message { get; set; }
}
