using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace FoodShop.BuildingBlocks.Configuration.Security;

public class ConfigureJWTBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;
    private readonly Lazy<RsaSecurityKey> _lazyRsaSecurityKey;

    public ConfigureJWTBearerOptions(IConfiguration configuration)
    {
        _configuration = configuration;
        _lazyRsaSecurityKey = new Lazy<RsaSecurityKey>(() =>
        {
            var rawKey = _configuration[ConfigurationJWTKeys.RsaKey] ?? throw new InvalidOperationException($"Configuration key \"{ConfigurationJWTKeys.RsaKey}\" was not found");
            var rsa = RSA.Create();
            rsa.FromXmlString(rawKey);
            return new RsaSecurityKey(rsa);
        });
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration[ConfigurationJWTKeys.ValidIssuer],
            ValidateAudience = true,
            ValidAudience = _configuration[ConfigurationJWTKeys.ValidAudience],
            IssuerSigningKey = _lazyRsaSecurityKey.Value,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
