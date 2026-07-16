using static Ecommerce.CartService.Api.Constants;

namespace Ecommerce.CartService.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCartAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthConstants.StoreCustomerManagerOnlyPolicy, policy =>
                policy.RequireRole(AuthConstants.ManagerRole, AuthConstants.StoreCustomerRole));

        return services;
    }
}
