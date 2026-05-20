namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface ITransactionManager
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
