namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface IOutboxService
    {
        Task AddOutboxMessageAsync(string payload, string eventType);

        Task ProcessPendingMessagesAsync(CancellationToken cancellationToken);
    }
}
