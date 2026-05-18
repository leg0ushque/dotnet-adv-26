using Ecommerce.CatalogService.Api;
using Ecommerce.CatalogService.Persistence.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.IntegrationTests.Fixtures
{
    public class CatalogWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<EcommerceCatalogDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<EcommerceCatalogDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<EcommerceCatalogDbContext>();

                db.Database.EnsureCreated();
            });
        }
    }
}
