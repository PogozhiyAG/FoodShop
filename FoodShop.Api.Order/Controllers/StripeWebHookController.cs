﻿using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Stripe;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodShop.MessageContracts.Order;
using Stripe;

namespace FoodShop.Api.Order.Controllers;

[ApiController]
[Route("[controller]")]
public class StripeWebHookController : ControllerBase
{
    private readonly IDbContextFactory<OrderDbContext> _dbContextFactory;
    private readonly IPublishEndpoint _publishEndpoint;

    public StripeWebHookController(IDbContextFactory<OrderDbContext> dbContextFactory, IPublishEndpoint publishEndpoint)
    {
        _dbContextFactory = dbContextFactory;
        _publishEndpoint = publishEndpoint;
    }

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
                using var db = await _dbContextFactory.CreateDbContextAsync();

                var orderPaymentIntent = await db.OrderPaymentIntents
                    .Include(pi => pi.Order)
                    .FirstOrDefaultAsync(pi => pi.PaymentIntentId == eventObject.Id);

                if(orderPaymentIntent?.Order != null)
                {
                    orderPaymentIntent.Order.Status = Model.OrderStatus.Paid;
                    await db.SaveChangesAsync();

                    await _publishEndpoint.Publish(new OrderPaid(
                        OrderId: orderPaymentIntent.Order.Id,
                        Moment: DateTime.Now
                    ));
                }
            }
        }

        return Ok();
    }
}
