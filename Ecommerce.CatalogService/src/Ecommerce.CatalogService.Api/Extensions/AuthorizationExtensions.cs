using Ecommerce.CatalogService.Api.Constants;

namespace Ecommerce.CatalogService.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCatalogAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthConstants.ManagerOnlyPolicy, policy =>
                policy.RequireRole(AuthConstants.ManagerRole))
            .AddPolicy(AuthConstants.MutatingAdminManagerPolicy, policy =>
                policy.RequireRole(AuthConstants.ManagerRole, AuthConstants.AdminRole));

        return services;
    }
}
