using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Services.Calculation;
using FoodShop.Api.Order.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Dto.Extensions;
using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentIntentController : ControllerBase
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly IDbContextFactory<OrderDbContext> _dbContextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderCalculator _orderCalculator;
        private readonly IProductCatalog _productCatalog;

        public PaymentIntentController(PaymentIntentService paymentIntentService, IDbContextFactory<OrderDbContext> dbContextFactory, IHttpContextAccessor httpContextAccessor, IOrderCalculator orderCalculator, IProductCatalog productCatalog)
        {
            _paymentIntentService = paymentIntentService;
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _orderCalculator = orderCalculator;
            _productCatalog = productCatalog;
        }


        private string GetUserName() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest createOrderRequest)
        {
            var userName = GetUserName();

            using var orderDbContext = _dbContextFactory.CreateDbContext();

            var existingCheckoutOrders = orderDbContext.Orders.Where(o => o.UserId == userName && o.Status == OrderStatus.Checkout).ToList();
            if(existingCheckoutOrders.Count > 0)
            {
                orderDbContext.Orders.RemoveRange(existingCheckoutOrders);
            }

            var order = createOrderRequest.ToOrder(o =>
            {
                o.Status = OrderStatus.Checkout;
                o.UserId = userName;
            });

            var result = await _orderCalculator.CalculateOrder(order);

            orderDbContext.Add(order);

            await orderDbContext.SaveChangesAsync();

            var amount = Convert.ToInt64(Math.Round(order.OrderCalculations.Sum(c => c.Amount) * 100));

            var paymentIntent = await _paymentIntentService.CreateAsync(new PaymentIntentCreateOptions()
            {
                Amount = amount,
                Currency = "gbp"
            });

            return Ok(new { paymentIntent.ClientSecret });
        }
    }
}
