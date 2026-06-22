using Ecommerce.CatalogService.Application.Common.DTOs;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Products.DTOs;

namespace Ecommerce.CatalogService.Application.Products.Interfaces;

public interface IProductService
    : IService<ProductDto, CreateProductDto, UpdateProductDto>
{
    public Task<PaginatedResult<ProductDto>> GetProductsAsync(string? categoryId, int pageNumber, int pageSize);
}
