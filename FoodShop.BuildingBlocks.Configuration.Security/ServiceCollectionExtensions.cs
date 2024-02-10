using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FoodShop.BuildingBlocks.Configuration.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFoodShopJwt(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJWTBearerOptions>();
        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        return services;
    }
}
