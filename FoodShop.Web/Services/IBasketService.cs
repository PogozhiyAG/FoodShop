using FoodShop.Core.Models;

namespace FoodShop.Web.Services;

public interface IBasketService
{
    void AddProduct(string basketId, Guid productId, decimal price);
    void RemoveProduct(string basketId, Guid productId);
    void SetProduct(string basketId, Guid productId, decimal price);
    Basket GetBasketById(string basketId);
}
