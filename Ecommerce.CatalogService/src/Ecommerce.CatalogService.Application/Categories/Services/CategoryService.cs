using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Interfaces;
using Ecommerce.CatalogService.Application.Common;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;

namespace Ecommerce.CatalogService.Application.Categories.Services
{
    public class CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
        : BaseService<Category, CategoryDto, CreateCategoryDto, UpdateCategoryDto>(categoryRepository, mapper), ICategoryService
    {
        public override void UpdateEntityDetails(Category entityToUpdate, UpdateCategoryDto updateDto) 
            => entityToUpdate.UpdateDetails(
                updateDto.Name,
                updateDto.ImageUrl,
                updateDto.ParentCategoryId);
    }
}
