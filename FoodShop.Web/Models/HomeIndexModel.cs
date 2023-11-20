using FoodShop.Core.Models;

namespace FoodShop.Web.Models;

public class HomeIndexModel
{
    public IEnumerable<ProductCategoryModel>? ProductCategories { get; set; }
    public IEnumerable<ProductCardModel>? ProductCards { get; set; }
}
