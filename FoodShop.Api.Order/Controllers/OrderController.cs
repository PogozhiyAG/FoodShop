using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Dto.Extensions;
using FoodShop.Api.Order.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.Api.Order.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController (IMediator _mediator) : ControllerBase
{
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody]CreateOrderRequest createOrderRequest)
    {
        var order = createOrderRequest.ToOrder(o =>
        {
            o.Status = OrderStatus.Draft;
        });

        var result = await _mediator.Send(new CalculateOrderCommand() { Order = order });
        return Ok(result);
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest createOrderRequest)
    {
        var command = new CreateCheckoutCommand()
        {
            CreateOrderRequest = createOrderRequest
        };

        var commandResult = await _mediator.Send(command);

        return Ok(new {
            commandResult.PaymentIntent!.ClientSecret
        });
    }
}

