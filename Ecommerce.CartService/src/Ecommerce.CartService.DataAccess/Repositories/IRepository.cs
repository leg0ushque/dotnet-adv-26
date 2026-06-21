using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.CartService.DataAccess.Repositories
{
    public interface IRepository<TEntity>
    {
        public Task<string> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField newValue,
        CancellationToken cancellationToken = default);
        Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);
        Task<List<TEntity>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
