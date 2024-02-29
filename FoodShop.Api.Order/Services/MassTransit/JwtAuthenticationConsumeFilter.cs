using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace FoodShop.Api.Order.Services.MassTransit;

public class JwtAuthenticationConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<JwtAuthenticationConsumeFilter<T>> _logger;
    private readonly IOptions<JwtBearerOptions> _jwtBearerOptions;
    private readonly IAuthenticationContext _authenticationContext;

    public JwtAuthenticationConsumeFilter(ILogger<JwtAuthenticationConsumeFilter<T>> logger, IAuthenticationContext authenticationContext, IOptions<JwtBearerOptions> jwtBearerOptions)
    {
        _logger = logger;
        _authenticationContext = authenticationContext;
        _jwtBearerOptions = jwtBearerOptions;
    }

    public void Probe(ProbeContext context)
    {

    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        _logger.LogInformation("Hello from consume filter!");

        if(context.TryGetHeader("token", out string? value))
        {
            _logger.LogInformation($"{value}");
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(value, _jwtBearerOptions.Value.TokenValidationParameters, out var securityToken);
                _authenticationContext.User = claimsPrincipal;
                _authenticationContext.Token = value;
            }
            catch
            {
            }

        }

        await next.Send(context);
    }
}
