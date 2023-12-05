using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace FoodShop.Api.CustomerProfile.Configuration;

public class ConfigureJWTBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureJWTBearerOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["JWT:ValidAudience"],
            IssuerSigningKey = GetKey(),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private RsaSecurityKey GetKey()
    {
        var raw = _configuration["JWT:PublicKey"];
        var rsa = RSA.Create();
        rsa.FromXmlString(raw);
        return new RsaSecurityKey(rsa);
    }
}
