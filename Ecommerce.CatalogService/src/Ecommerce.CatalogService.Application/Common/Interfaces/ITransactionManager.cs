namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface ITransactionManager
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
