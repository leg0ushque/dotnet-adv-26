namespace Ecommerce.CatalogService.Api.Models;

public class ResourceResponse<T>
{
    public T Data { get; set; } = default!;
    public List<Link> Links { get; set; } = new();
}
