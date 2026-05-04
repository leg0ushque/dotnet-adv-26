using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public interface IService<TEntity, TDto>
    {
        Task CreateAsync(TDto dto, CancellationToken cancellationToken = default);
        Task<List<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
