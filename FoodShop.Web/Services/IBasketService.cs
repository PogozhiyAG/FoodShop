using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pipelines.Sockets.Unofficial.Buffers;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace FoodShop.Web.Services;

public interface IBasketService
{
    void AddItem(string ownerId, int productId, int quantity);
    void SetItem(string ownerId, int productId, int quantity);
    Basket GetBasket(string ownerId);
}


public class RedisBasketService : IBasketService
{
    public void AddItem(string ownerId, int productId, int quantity)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var (basket, basketItem) = EnsureBasketItem(ownerId, productId);
        basketItem.Quantity = basketItem.Quantity + quantity;
        basket.RemoveEmptyItems();
        SaveBasket(basket);
        sw.Stop();
        Console.WriteLine(sw.Elapsed);
    }

    public void SetItem(string ownerId, int productId, int quantity)
    {
        var (basket, basketItem) = EnsureBasketItem(ownerId, productId);
        basketItem.Quantity = quantity;
        basket.RemoveEmptyItems();
        SaveBasket(basket);
    }

    public Basket GetBasket(string ownerId)
    {
        return EnsureBasketByOwnerId(ownerId);
    }

    private (Basket, BasketItem) EnsureBasketItem(string basketId, int productId)
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

    private string GetBasketCacheKey(string ownerId) => $"basket-{ownerId}";

    private Basket EnsureBasketByOwnerId(string ownerId)
    {
        var connection = ConnectionMultiplexer.Connect("basketdata");
        var cache = connection.GetDatabase();

        Basket basket = null;
        var entry = cache.StringGet(GetBasketCacheKey(ownerId));
        if (entry.IsNullOrEmpty)
        {
            basket = new() { OwnerId = ownerId };
            cache.StringSet(GetBasketCacheKey(ownerId), JsonSerializer.Serialize(basket));
        }
        else
        {
            basket = JsonSerializer.Deserialize<Basket>(entry);
        }
        return basket;
    }

    private void SaveBasket(Basket basket)
    {
        var connection = ConnectionMultiplexer.Connect("basketdata");
        var cache = connection.GetDatabase();
        cache.StringSet(GetBasketCacheKey(basket.OwnerId), JsonSerializer.Serialize(basket));
    }
}


public class BasketService : IBasketService
{
    private readonly FoodShopDbContext _context;

    public BasketService(FoodShopDbContext context)
    {
        _context = context;
    }

    public void AddItem(string ownerId, int productId, int quantity)
    {
        var (basket, basketItem) = EnsureBasketItem(ownerId, productId);
        basketItem.Quantity = basketItem.Quantity + quantity;
        basket.RemoveEmptyItems();
        _context.SaveChanges();
    }


    public void SetItem(string ownerId, int productId, int quantity)
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

    private (Basket, BasketItem) EnsureBasketItem(string basketId, int productId)
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
