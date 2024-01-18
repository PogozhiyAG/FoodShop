using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services;
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
    IDbContextFactory<OrderDbContext> _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOrderCalculator _orderCalculator;
    private readonly IProductCatalog _productCatalog;

    public OrderController(IDbContextFactory<OrderDbContext> dbContextFactory, IHttpContextAccessor httpContextAccessor, IOrderCalculator orderCalculator, IProductCatalog productCatalog)
    {
        _dbContextFactory = dbContextFactory;
        _httpContextAccessor = httpContextAccessor;
        _orderCalculator = orderCalculator;
        _productCatalog = productCatalog;
    }

    private string GetUserName() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        using var orderDbContext = _dbContextFactory.CreateDbContext();
        var order = orderDbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.OrderCalculations)
                .ThenInclude(c => c.Properties)
            .Where(o => o.Id == id)
            .FirstOrDefault();

        if (order == null)
        {
            return NotFound();
        }

        //TODO: Draft. rename OrderCalculationContext => smt else
        var ctx = new OrderCalculationContext() { Order = order };
        ctx.CalculatedOrderItems = await _productCatalog.CalculateProducts(order.Items.Select(i => new ValueTuple<string, int>(i.ProductId, i.Quantity)));

        return Ok(ctx);
    }


    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest createOrderRequest)
    {
        var order = createOrderRequest.ToOrder(o =>
        {
            o.Status = OrderStatus.Created;
            o.UserId = GetUserName();
        });

        var orderCalculationCtx = await _orderCalculator.CalculateOrder(order);

        using var orderDbContext = _dbContextFactory.CreateDbContext();
        orderDbContext.Add(order);
        await orderDbContext.SaveChangesAsync();

        //TODO: Created?
        return Ok(orderCalculationCtx);
    }


    [HttpGet("calculate")]
    public async Task<IActionResult> Calculate([FromBody]CreateOrderRequest createOrderRequest)
    {
        var order = createOrderRequest.ToOrder(o =>
        {
            o.Status = OrderStatus.Draft;
        });

        var orderCalculationCtx = await _orderCalculator.CalculateOrder(order);
        return Ok(orderCalculationCtx);
    }
}

