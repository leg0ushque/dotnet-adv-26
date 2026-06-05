using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Persistence.BackgroundServices;
using Ecommerce.CatalogService.Persistence.Configuration;
using Ecommerce.CatalogService.Persistence.Data;
using Ecommerce.CatalogService.Persistence.Extensions;
using Ecommerce.CatalogService.Persistence.Messaging;
using Ecommerce.CatalogService.Persistence.Repositories;
using Ecommerce.CatalogService.Persistence.Services;
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
            bool isTesting = false,
            bool outboxServiceEnabled = false)
        {
            if (isTesting)
            {
                services.AddDbContext<EcommerceCatalogDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                    options.EnableServiceProviderCaching(false);
                });

                services.AddSingleton<IMessagePublisher, FakeMessagePublisher>();
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<EcommerceCatalogDbContext>(options =>
                    options.UseSqlServer(
                        connectionString,
                        b => b.MigrationsAssembly(typeof(EcommerceCatalogDbContext).Assembly.FullName)));

                // RabbitMQ

                services.RegisterOptions<RabbitMqOptions>(RabbitMqOptions.SectionName);
                services.AddSingleton<RabbitMqConnectionFactory>();
                services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
                services.AddHostedService<RabbitMqTopologyInitializer>();

                if(outboxServiceEnabled)
                    services.AddHostedService<OutboxProcessorBackgroundService>();
            }

            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<EcommerceCatalogDbContext>());

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ITransactionManager, CatalogTransactionManager>();
			
            services.AddScoped<IOutboxService, OutboxService>();

            return services;
        }
    }

    internal sealed class FakeMessagePublisher : IMessagePublisher
    {
        public Task PublishAsync(string serializedBody, string eventTypeName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
