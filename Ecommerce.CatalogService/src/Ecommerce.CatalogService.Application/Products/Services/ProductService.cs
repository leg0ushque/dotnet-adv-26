using AutoMapper;
using Ecommerce.CatalogService.Application.Common;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Products.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Products.Services
{
    public class ProductService(IRepository<Product> productRepository, 
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        IMapper mapper)
        : BaseService<Product, ProductDto, CreateProductDto, UpdateProductDto>(
            productRepository, 
            createValidator, 
            updateValidator, 
            mapper), IProductService
    {
        public override void UpdateEntityDetails(Product entityToUpdate, UpdateProductDto updateDto)
            => entityToUpdate.UpdateDetails(
                updateDto.Name, 
                updateDto.CategoryId, 
                updateDto.Price, 
                updateDto.Amount, 
                updateDto.Description, 
                updateDto.ImageUrl);
    }
}
