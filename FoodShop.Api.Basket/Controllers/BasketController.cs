using FoodShop.Api.Basket.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace FoodShop.Api.Basket.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class BasketController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BasketController(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private string GetBasketId() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;
    private string GetBasketCacheKey() => $"basket/{GetBasketId()}";

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        //TODO edge case when the basket is empty
        var basket = await EnsureBasket();
        return Ok(basket);
    }


    [HttpPost("add")]
    public async Task<IActionResult> Add(string product, int qty = 1)
    {
        CustomerBasket basket = await EnsureBasket();
        basket.Items.AddQuantity(product, qty);
        await SaveBasket(basket);
        return Ok(basket);
    }


    [HttpPost("set")]
    public async Task<IActionResult> Set(string product, int qty)
    {
        CustomerBasket basket = await EnsureBasket();
        basket.Items.SetQuantity(product, qty);
        await SaveBasket(basket);
        return Ok(basket);
    }


    [HttpPost("clear")]
    public async Task<IActionResult> Clear()
    {
        var key = GetBasketCacheKey();
        await _cache.RemoveAsync(key);
        return Ok();
    }


    private async Task<CustomerBasket> EnsureBasket()
    {
        var key = GetBasketCacheKey();
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

    private async Task SaveBasket(CustomerBasket basket)
    {
        var key = GetBasketCacheKey();
        if (basket.Items.Any())
        {
            var value = JsonSerializer.Serialize(basket);
            await _cache.SetStringAsync(key, value);
        }
        else
        {
            await _cache.RemoveAsync(key);
        }
    }


    private async Task<string> GetCalculatedBasket(CustomerBasket basket)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var url = _configuration["ApiUrls:FoodShop.Api.Catalog"];

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Headers =
            {
                { HeaderNames.Authorization, Request.Headers.Authorization.FirstOrDefault() }
            },
            Content = JsonContent.Create(basket)
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        var result = await httpResponseMessage.Content.ReadAsStringAsync();

        return result;
    }
}
