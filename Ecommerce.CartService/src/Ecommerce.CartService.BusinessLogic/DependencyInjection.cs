using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Services;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CartService.BusinessLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddScoped<ICartService, Services.CartService>();

            services.AddScoped<ICreateValidator<CartDto>, CartDtoCreateValidator>();
            services.AddScoped<IUpdateValidator<CartDto>, CartDtoUpdateValidator>();
            services.AddScoped<ICreateValidator<CartItemDto>, CartItemDtoValidator>();

            services.AddDataAccessServices(connectionString, databaseName);

            return services;
        }
    }

}