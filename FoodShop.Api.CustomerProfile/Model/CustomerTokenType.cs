using System.ComponentModel.DataAnnotations;

namespace FoodShop.Api.CustomerProfile.Model;

public class CustomerTokenType
{
    [Key]
    public string Code { get; set; }
    public string? Description { get; set; }
}
