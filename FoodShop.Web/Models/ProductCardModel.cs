using FoodShop.Core.Models;

namespace FoodShop.Web.Models;

public class ProductCardModel
{
    public Product Product { get; set; }
    public IEnumerable<ProductPriceStrategyLink> Offers { get; set; }
}
