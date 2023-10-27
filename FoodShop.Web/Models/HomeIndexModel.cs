using FoodShop.Core.Models;

namespace FoodShop.Web.Models;

public class HomeIndexModel
{
    public IEnumerable<ProductCategory>? ProductCategories { get; set; }
    public IEnumerable<Product>? Product { get; set; }
}
