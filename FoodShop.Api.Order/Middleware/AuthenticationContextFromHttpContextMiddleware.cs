using FoodShop.Api.Order.Services;

namespace FoodShop.Api.Order.Middleware;

public class AuthenticationContextFromHttpContextMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationContextFromHttpContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthenticationContext authenticationContext)
    {
        authenticationContext.User = context.User;

        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authorizationHeader != null)
        {
            var token = authorizationHeader!.Substring("Bearer ".Length);
            authenticationContext.Token = token;
        }

        await _next.Invoke(context);
    }
}
