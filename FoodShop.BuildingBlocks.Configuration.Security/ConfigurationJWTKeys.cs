namespace FoodShop.BuildingBlocks.Configuration.Security;

public static class ConfigurationJWTKeys
{
    public const string Section = "JWT";

    public const string ValidIssuer = $"{Section}:ValidIssuer";
    public const string ValidAudience = $"{Section}:ValidAudience";
    public const string RsaKey = $"{Section}:RsaKey";
}
