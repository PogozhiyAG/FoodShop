using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Web.Services;

public interface IBasketService
{
    void AddItem(string ownerId, Guid productId, int quantity);
    void SetItem(string ownerId, Guid productId, int quantity);
    Basket GetBasket(string ownerId);
}

public class BasketService : IBasketService
{
    private readonly FoodShopDbContext _context;

    public BasketService(FoodShopDbContext context)
    {
        _context = context;
    }

    public void AddItem(string ownerId, Guid productId, int quantity)
    {
        var (basket, basketItem) = EnsureBasketItem(ownerId, productId);
        basketItem.Quantity = basketItem.Quantity + quantity;
        basket.RemoveEmptyItems();
        _context.SaveChanges();
    }


    public void SetItem(string ownerId, Guid productId, int quantity)
    {
        var (basket, basketItem) = EnsureBasketItem(ownerId, productId);
        basketItem.Quantity = quantity;
        basket.RemoveEmptyItems();
        _context.SaveChanges();
    }

    public Basket GetBasket(string ownerId)
    {
        return EnsureBasketByOwnerId(ownerId);
    }

    private (Basket, BasketItem) EnsureBasketItem(string basketId, Guid productId)
    {
        var basket = EnsureBasketByOwnerId(basketId);

        var basketItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (basketItem == null)
        {
            basketItem = new() { ProductId = productId };
            basket.Items.Add(basketItem);
        }

        return (basket, basketItem);
    }

    private Basket EnsureBasketByOwnerId(string ownerId)
    {
        var basket = _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefault(b => b.OwnerId == ownerId);
        if (basket == null)
        {
            basket = new Basket() { OwnerId = ownerId };
            _context.Baskets.Add(basket);
        }
        return basket;
    }
}
