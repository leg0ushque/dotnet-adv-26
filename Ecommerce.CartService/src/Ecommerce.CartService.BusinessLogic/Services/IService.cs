using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.CartService.BusinessLogic.Results;

namespace Ecommerce.CartService.BusinessLogic.Services;

public interface IService<TEntity, TDto>
{
    public Task<Result<string>> CreateAsync(TDto dto);
    public Task<List<TDto>> GetAllAsync();
    public Task<Result> DeleteAsync(string id);
}
