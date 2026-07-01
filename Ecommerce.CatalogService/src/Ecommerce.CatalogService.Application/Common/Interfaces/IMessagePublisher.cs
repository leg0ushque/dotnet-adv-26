namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface IMessagePublisher
{
    public Task PublishAsync(string serializedBody, string eventTypeName,
        CancellationToken cancellationToken = default);
}
