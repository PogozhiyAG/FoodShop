using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services.Calculation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Order.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _orderDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOrderCalculator _orderCalculator;

    public OrderController(OrderDbContext orderDbContext, IHttpContextAccessor httpContextAccessor, IOrderCalculator orderCalculator)
    {
        _orderDbContext = orderDbContext;
        _httpContextAccessor = httpContextAccessor;
        _orderCalculator = orderCalculator;
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

        _orderDbContext.Add(order);

        var orderCalculations = (await _orderCalculator.CalculateOrder(order)).ToList();
        _orderDbContext.AddRange(orderCalculations);

        await _orderDbContext.SaveChangesAsync();

        //TODO: Created?
        return Ok(order.Id);
    }
}

