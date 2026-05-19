using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Persistence.Data;
using Ecommerce.CatalogService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services, 
            IConfiguration configuration,
            bool useInMemoryDatabase = false)
        {
            if (useInMemoryDatabase)
            {
                // Testing configuration: InMemory database
                services.AddDbContext<EcommerceCatalogDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                    options.EnableServiceProviderCaching(false);
                });
            }
            else
            {
                // Production configuration: SQL Server
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<EcommerceCatalogDbContext>(options =>
                    options.UseSqlServer(
                        connectionString,
                        b => b.MigrationsAssembly(typeof(EcommerceCatalogDbContext).Assembly.FullName)));
            }

            // Common services for both configurations
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<EcommerceCatalogDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
