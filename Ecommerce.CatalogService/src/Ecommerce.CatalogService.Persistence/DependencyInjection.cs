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
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<EcommerceCatalogDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(EcommerceCatalogDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<EcommerceCatalogDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
