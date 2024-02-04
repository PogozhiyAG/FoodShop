using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace FoodShop.Api.Order.Controllers;

[ApiController]
[Route("[controller]")]
public class StripeWebHookController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json, false);

            // Handle the event
            Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            Console.WriteLine($"Id: {(stripeEvent.Data.Object as IHasId).Id}");

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }
}
