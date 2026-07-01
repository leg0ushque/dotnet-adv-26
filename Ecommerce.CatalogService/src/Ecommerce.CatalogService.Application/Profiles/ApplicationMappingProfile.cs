using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Common.DTOs.QueueMessages;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Domain.Entities;

namespace Ecommerce.CatalogService.Application.Profiles;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CreateCategoryDto, Category>().ReverseMap();
        CreateMap<UpdateCategoryDto, Category>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();

        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();
        CreateMap<Product, ProductDto>().ReverseMap();

        CreateMap<UpdateProductDto, UpdatedProductMessage>();
    }
}
