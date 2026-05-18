using Ecommerce.CatalogService.Application.Common.Results;

namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface IService<TDto, TCreateDto, TUpdateDto>
        where TCreateDto : class
        where TUpdateDto : class
        where TDto : class
    {
        Task<List<TDto>> GetAllAsync();
        Task<Result<TDto>> GetByIdAsync(string id);
        Task<Result<string>> CreateAsync(TCreateDto dto);
        Task<Result> UpdateAsync(string id, TUpdateDto dto);
        Task<Result> DeleteAsync(string id);
    }
}
