using Ecommerce.CatalogService.Application.Common.Results;

namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface IService<TDto, TCreateDto, TUpdateDto>
    where TCreateDto : class
    where TUpdateDto : class
    where TDto : class
{
    public Task<List<TDto>> GetAllAsync();
    public Task<Result<TDto>> GetByIdAsync(string id);
    public Task<Result<string>> CreateAsync(TCreateDto dto);
    public Task<Result> UpdateAsync(string id, TUpdateDto dto);
    public Task<Result> DeleteAsync(string id);
}
