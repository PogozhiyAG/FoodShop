using FoodShop.Api.Basket.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FoodShop.Api.Basket.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class BasketController : ControllerBase
{
    private readonly IDistributedCache _cache;

    public BasketController(IDistributedCache cache)
    {
        _cache = cache;
    }

    private string GetBasketCacheKey(string id) => $"basket/{id}";

    [HttpGet]
    public async Task<IActionResult> Get(string id)
    {
        var basket = await EnsureBasket(id);
        return Ok(basket);
    }


    [HttpPost("add")]
    public async Task<IActionResult> Add(string id, string product, int qty = 1)
    {
        CustomerBasket basket = await EnsureBasket(id);

        basket.Items.AddQuantity(product, qty);

        SaveBasket(id, basket);

        return Ok(basket);
    }


    [HttpPost("set")]
    public async Task<IActionResult> Set(string id, string product, int qty)
    {
        CustomerBasket basket = await EnsureBasket(id);

        basket.Items.SetQuantity(product, qty);

        SaveBasket(id, basket);

        return Ok(basket);
    }


    private async Task<CustomerBasket> EnsureBasket(string id)
    {
        var key = GetBasketCacheKey(id);
        var value = await _cache.GetStringAsync(key);

        CustomerBasket basket;
        if (value != null)
        {
            basket = JsonSerializer.Deserialize<CustomerBasket>(value)!;
        }
        else
        {
            basket = new CustomerBasket();
        }

        return basket;
    }

    private void SaveBasket(string id, CustomerBasket basket)
    {
        var key = GetBasketCacheKey(id);
        var value = JsonSerializer.Serialize(basket);
        _cache.SetString(key, value);
    }
}
