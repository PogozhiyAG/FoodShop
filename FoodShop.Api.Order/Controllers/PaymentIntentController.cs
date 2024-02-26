using Microsoft.AspNetCore.Mvc;
using FoodShop.Api.Order.Dto;
using MediatR;
using FoodShop.Api.Order.Commands;

namespace FoodShop.Api.Order.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentIntentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public PaymentIntentController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        private string GetUserName() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest createOrderRequest)
        {
            var command = new CreateCheckoutCommand() {
                UserId = GetUserName(),
                CreateOrderRequest = createOrderRequest
            };

            var commandResult = await _mediator.Send(command);

            return Ok(new {
                commandResult.PaymentIntent!.ClientSecret
            });
        }
    }
}
