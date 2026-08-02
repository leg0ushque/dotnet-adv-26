using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Ecommerce.ApiGateway.WebApi
{
    public class KeycloakRolesTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
                return Task.FromResult(principal);

            var realmAccess = identity.FindFirst("realm_access");
            if (realmAccess is null) return Task.FromResult(principal);

            using var doc = JsonDocument.Parse(realmAccess.Value);
            if (!doc.RootElement.TryGetProperty("roles", out var roles))
                return Task.FromResult(principal);

            foreach (var role in roles.EnumerateArray())
            {
                var value = role.GetString();
                if (value is null) continue;

                // Both claim types needed: ClaimTypes.Role for ASP.NET, "role" for Ocelot
                if (!identity.HasClaim(ClaimTypes.Role, value))
                    identity.AddClaim(new Claim(ClaimTypes.Role, value));

                if (!identity.HasClaim("role", value))
                    identity.AddClaim(new Claim("role", value));
            }

            return Task.FromResult(principal);
        }
    }
}
