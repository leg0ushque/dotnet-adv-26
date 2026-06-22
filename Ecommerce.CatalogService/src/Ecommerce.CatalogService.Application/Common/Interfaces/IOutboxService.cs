namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface IOutboxService
{
    public Task AddOutboxMessageAsync(string payload, string eventType);

    public Task ProcessPendingMessagesAsync(CancellationToken cancellationToken);
}
