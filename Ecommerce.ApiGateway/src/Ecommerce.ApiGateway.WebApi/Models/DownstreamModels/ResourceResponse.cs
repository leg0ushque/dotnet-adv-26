namespace Ecommerce.ApiGateway.WebApi.Models.Downstream;

public class ResourceResponse<T>
{
    public T Data { get; set; } = default!;
    public List<Link> Links { get; set; } = new();
}
