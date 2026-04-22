using Ecommerce.CatalogService.Application.Categories.Interfaces;
using Ecommerce.CatalogService.Application.Categories.Services;
using Ecommerce.CatalogService.Application.Categories.Validators;
using Ecommerce.CatalogService.Application.Products.Interfaces;
using Ecommerce.CatalogService.Application.Products.Services;
using Ecommerce.CatalogService.Application.Profiles;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

            services.AddAutoMapper(x => x.AddProfile<ApplicationMappingProfile>());

            return services;
        }
    }
}
