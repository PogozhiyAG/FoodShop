using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Dto.Catalog;
using FoodShop.Api.Order.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Net.Http;

namespace FoodShop.Api.Order.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _orderDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OrderController(OrderDbContext orderDbContext, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _orderDbContext = orderDbContext;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private string GetUserName() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;

    [HttpGet]
    public IActionResult Get(Guid id)
    {
        var order = _orderDbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.Id == id)
            .FirstOrDefault();

        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateOrderRequest createOrderRequest)
    {
        var now = DateTime.UtcNow;

        Model.Order order = new()
        {
            Id = Guid.NewGuid(),
            //TODO: Name or ID
            UserId = GetUserName(),
            CreateDate = now,
            Status = Model.OrderStatus.Created,
            Description = createOrderRequest.Description,
            Items = new List<OrderItem>()
        };

        foreach (var requestOrderItem in createOrderRequest.Items)
        {
            order.Items.Add(new Model.OrderItem()
            {
                Order = order,
                ProductId = requestOrderItem.ProductId,
                Quantity = requestOrderItem.Quantity
            });
        }

        if (createOrderRequest.Delivery != null)
        {
            order.DeliveryInfo = new()
            {
                Id = Guid.NewGuid(),
                Address = createOrderRequest.Delivery.Address,
                ContactName = createOrderRequest.Delivery.ContactName,
                ContactPhone = createOrderRequest.Delivery.ContactPhone,
                TimeSlotFrom = createOrderRequest.Delivery.TimeSlotFrom,
                TimeSlotTo = createOrderRequest.Delivery.TimeSlotTo
            };
        }


        var customerBasket = new ProductBatchCalculationRequest();
        customerBasket.Items = order.Items.ToDictionary(i => i.ProductId, i => i.Quantity);

        var calculatedProducts = await CalculateProducts(customerBasket);

        foreach (var calculatedProduct in calculatedProducts)
        {
            var orderCalculation = new OrderCalculation()
            {
                Order = order,
                TypeCode = OrderCalculationTypeCodes.Product,
                CreateDate = now,
                Amount = calculatedProduct.OfferAmount
            };
            _orderDbContext.Add(orderCalculation);
        }

        _orderDbContext.Add(order);
        await _orderDbContext.SaveChangesAsync();

        //TODO: Created?
        return Ok(order.Id);
    }

    private async Task<List<CalculatedOrderItem>> CalculateProducts(ProductBatchCalculationRequest basket)
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

        var result = await httpResponseMessage.Content.ReadFromJsonAsync<List<CalculatedOrderItem>>();

        return result;
    }
}

public class ProductBatchCalculationRequest
{
    public Dictionary<string, int> Items { get; set; } = new();

}