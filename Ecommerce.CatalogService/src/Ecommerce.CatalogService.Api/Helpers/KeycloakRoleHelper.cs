using Ecommerce.CatalogService.Api.Constants;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Ecommerce.CatalogService.Api.Helpers
{
    public static class KeycloakRoleHelper
    {
        public static void MapKeycloakRolesToClaims(ClaimsIdentity identity)
        {
            var realmAccessClaim = identity.FindFirst(AuthConstants.RealmAccess)?.Value;

            if (string.IsNullOrEmpty(realmAccessClaim))
            {
                return;
            }

            var realmAccess = JObject.Parse(realmAccessClaim);

            if (realmAccess[AuthConstants.Roles] is JArray roles)
            {
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }
        }
    }
}
