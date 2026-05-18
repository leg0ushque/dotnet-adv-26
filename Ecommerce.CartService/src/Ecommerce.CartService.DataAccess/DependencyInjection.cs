using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using Ecommerce.CartService.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CartService.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString, string databaseName)
        {
            return services
                .AddSingleton<IMongoDbFactory>(new MongoDbFactory(connectionString, databaseName))
                .AddTransient<IRepository<Cart>, CartRepository>();
        }
    }
}