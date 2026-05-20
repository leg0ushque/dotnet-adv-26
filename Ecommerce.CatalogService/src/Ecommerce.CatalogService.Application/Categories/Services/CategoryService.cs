using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Interfaces;
using Ecommerce.CatalogService.Application.Common;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Common.Results;
using Ecommerce.CatalogService.Domain.Entities;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Categories.Services
{
    public class CategoryService(IRepository<Category> categoryRepository,
        IRepository<Product> productRepository,
        IValidator<CreateCategoryDto> createValidator, 
        IValidator<UpdateCategoryDto> updateValidator, 
        IMapper mapper,
        ITransactionManager transactionManager)
        : BaseService<Category, CategoryDto, CreateCategoryDto, UpdateCategoryDto>(categoryRepository, createValidator, updateValidator, mapper, transactionManager), ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository = categoryRepository;
        private readonly IRepository<Product> _productRepository = productRepository;

        protected override string EntityName => "Category";

        public override void UpdateEntityDetails(Category entityToUpdate, UpdateCategoryDto updateDto) 
            => entityToUpdate.UpdateDetails(
                updateDto.Name,
                updateDto.ImageUrl,
                updateDto.ParentCategoryId);

        public async Task<Result> DeleteCategoryWithProductsAsync(string id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return Result.Failure(Error.NotFound(EntityName, id));

            try
            {
                await _transactionManager.BeginTransactionAsync();

                var productsInCategory = await _productRepository.GetAllAsync(p => p.CategoryId == id);

                foreach (var product in productsInCategory)
                {
                    await _productRepository.DeleteByIdAsync(product.Id);
                }

                await _categoryRepository.DeleteByIdAsync(id);

                await _transactionManager.SaveChangesAsync();
                await _transactionManager.CommitTransactionAsync();

                return Result.Success();
            }
            catch
            {
                await _transactionManager.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
