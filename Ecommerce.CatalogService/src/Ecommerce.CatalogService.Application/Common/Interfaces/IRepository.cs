using System.Linq.Expressions;

namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    public Task<string> CreateAsync(TEntity item);
    public Task DeleteByIdAsync(string id);
    public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
    public Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter);
    public Task<TEntity?> GetByIdAsync(string id);
    public Task UpdateAsync(TEntity item);
}
