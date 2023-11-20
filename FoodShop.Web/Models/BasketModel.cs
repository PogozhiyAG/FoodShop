using FoodShop.Core.Models;
using FoodShop.Web.Services;

namespace FoodShop.Web.Models;

public class BasketModel
{
    public List<BasketItemModel> Items { get; set; }
    public decimal TotalAmount { get; set; }
}

public class BasketItemModel
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal CalculatedPrice { get; set; }
    public ProductPriceStrategyLink ProductPriceStrategyLink { get; set; }
}


