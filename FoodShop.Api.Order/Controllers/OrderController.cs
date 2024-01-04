using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Dto;
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

    public OrderController(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

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
    public IActionResult Create([FromBody]CreateOrderRequest createOrderRequest)
    {
        Console.WriteLine(createOrderRequest);
        return Created();
    }
}
