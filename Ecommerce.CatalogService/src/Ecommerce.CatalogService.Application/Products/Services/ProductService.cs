using AutoMapper;
using Ecommerce.CatalogService.Application.Common;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Products.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;

namespace Ecommerce.CatalogService.Application.Products.Services
{
    public class ProductService(IRepository<Product> productRepository, IMapper mapper)
        : BaseService<Product, ProductDto, CreateProductDto, UpdateProductDto>(productRepository, mapper), IProductService
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
