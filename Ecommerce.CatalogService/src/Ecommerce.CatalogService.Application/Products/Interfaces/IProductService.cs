using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Products.DTOs;

namespace Ecommerce.CatalogService.Application.Products.Interfaces
{
    public interface IProductService 
        : IService<ProductDto, CreateProductDto, UpdateProductDto>
    { }
}
