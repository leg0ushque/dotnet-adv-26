using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Common.Interfaces;

namespace Ecommerce.CatalogService.Application.Categories.Interfaces
{
    public interface ICategoryService 
        : IService<CategoryDto, CreateCategoryDto, UpdateCategoryDto>
    { }
}
