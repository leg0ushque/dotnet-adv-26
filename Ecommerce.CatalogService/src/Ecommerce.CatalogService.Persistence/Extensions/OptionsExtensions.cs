using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ecommerce.CatalogService.Persistence.Extensions;

public static class OptionsExtensions
{
    public static OptionsBuilder<TOptions> RegisterOptions<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        return services.AddOptions<TOptions>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(sectionName).Bind(settings);
            });
    }
}
