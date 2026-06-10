using System.Security.Claims;

namespace Ecommerce.CartService.Api.Middleware
{
    public class IdentityLoggingMiddleware(RequestDelegate next, ILogger<IdentityLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<IdentityLoggingMiddleware> _logger = logger;

        private const string _subClaim = "sub";
        private const string _preferredUsernameClaim = "preferred_username";
        private const string _unknownValue = "Unknown";

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var claims = context.User.Claims.Select(c => new { c.Type, c.Value });
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst(_subClaim)?.Value
                ?? _unknownValue;
            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value
                ?? context.User.FindFirst(_preferredUsernameClaim)?.Value
                ?? _unknownValue;
            var roles = context.User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            _logger.LogInformation(
                "Authenticated request: UserId={UserId}, UserName={UserName}, Roles={Roles}, Path={Path}, Method={Method}",
                userId,
                userName,
                string.Join(", ", roles),
                context.Request.Path,
                context.Request.Method);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("All claims: {Claims}", Newtonsoft.Json.JsonConvert.SerializeObject(claims));
            }

            await _next(context);
        }
    }
}
