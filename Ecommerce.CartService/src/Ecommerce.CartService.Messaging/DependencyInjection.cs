using Ecommerce.CartService.Messaging.Configuration;
using Ecommerce.CartService.Messaging.Consumers;
using Ecommerce.CartService.Messaging.Handlers;
using Ecommerce.CartService.Messaging.Infrastructure;
using Ecommerce.CartService.Messaging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CartService.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessagingServices(
            this IServiceCollection services)
        {
            services.AddOptions<RabbitMqOptions>()
                .Configure<IConfiguration>((settings, config) =>
                {
                    config.GetSection(RabbitMqOptions.SectionName).Bind(settings);
                });

            services.AddHostedService<UpdatedProductConsumer>();

            services.AddSingleton<RabbitMqConnectionFactory>();
            services.AddHostedService<RabbitMqTopologyInitializer>();

            services.AddSingleton<IMessageHandler<UpdatedProductMessage>, UpdatedProductHandler>();

            return services;
        }
    }
}
