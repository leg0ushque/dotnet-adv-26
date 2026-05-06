using Ecommerce.CartService.BusinessLogic.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public interface IService<TEntity, TDto>
    {
        Task<Result<string>> CreateAsync(TDto dto);
        Task<List<TDto>> GetAllAsync();
        Task<Result> DeleteAsync(string id);
    }
}
