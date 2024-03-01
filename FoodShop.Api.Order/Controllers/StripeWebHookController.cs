using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Stripe;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using MediatR;
using FoodShop.Api.Order.Commands;

namespace FoodShop.Api.Order.Controllers;

[ApiController]
[Route("[controller]")]
public class StripeWebHookController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ParseEvent(json, false);

        //TODO: add flexibility
        if (stripeEvent.Type == StripeEventTypes.PAYMENT_INTENT_SUCCEEDED)
        {
            if(stripeEvent.Data.Object is IHasId eventObject)
            {

                var command = new OrderSetStatusPaidCommand() { PaymentIntentId = eventObject.Id };
                await _mediator.Send(command);
            }
        }

        return Ok();
    }
}
