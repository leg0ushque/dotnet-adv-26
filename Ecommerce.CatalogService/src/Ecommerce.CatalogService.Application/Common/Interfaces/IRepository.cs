using System.Linq.Expressions;

namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<string> CreateAsync(TEntity item);
        Task DeleteByIdAsync(string id);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetByIdAsync(string id);
        Task UpdateAsync(TEntity item);
    }
}
