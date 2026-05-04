using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.CartService.DataAccess.Repositories
{
    public interface IRepository<TEntity>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
