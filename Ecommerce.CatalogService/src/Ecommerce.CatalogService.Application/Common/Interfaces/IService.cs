using Ecommerce.CatalogService.Application.Products.DTOs;

namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface IService<TDto, TCreateDto, TUpdateDto>
        where TCreateDto : class
        where TUpdateDto : class
        where TDto : class
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(TCreateDto dto);
        Task UpdateAsync(string id, TUpdateDto dto);
        Task DeleteAsync(string id);
    }
}
