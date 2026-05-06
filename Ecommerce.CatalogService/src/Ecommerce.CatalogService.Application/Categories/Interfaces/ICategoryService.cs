using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Common.Results;

namespace Ecommerce.CatalogService.Application.Categories.Interfaces
{
    public interface ICategoryService 
        : IService<CategoryDto, CreateCategoryDto, UpdateCategoryDto>
    {
        Task<Result> DeleteCategoryWithProductsAsync(string id);
    }
}
