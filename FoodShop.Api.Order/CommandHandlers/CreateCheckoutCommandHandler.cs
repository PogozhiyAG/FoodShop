using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Dto.Extensions;
using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace FoodShop.Api.Order.CommandHandlers;

public class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, CreateCheckoutCommandResult>
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly IDbContextFactory<OrderDbContext> _dbContextFactory;
    private readonly IMediator _mediator;
    private readonly IAuthenticationContext _authenticationContext;

    public CreateCheckoutCommandHandler(PaymentIntentService paymentIntentService, IDbContextFactory<OrderDbContext> dbContextFactory, IMediator mediator, IAuthenticationContext authenticationContext)
    {
        _paymentIntentService = paymentIntentService;
        _dbContextFactory = dbContextFactory;
        _mediator = mediator;
        _authenticationContext = authenticationContext;
    }

    public async Task<CreateCheckoutCommandResult> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        CreateCheckoutCommandResult result = new();

        using var db = await _dbContextFactory.CreateDbContextAsync();

        result.Order = request.CreateOrderRequest.ToOrder(o =>
        {
            o.Status = OrderStatus.Checkout;
            o.UserId = _authenticationContext.User.Identity.Name;
        });

        result.OrderCalculationContext = await _mediator.Send(new CalculateOrderCommand() { Order = result.Order });
        db.Add(result.Order);
        await db.SaveChangesAsync();

        var amount = Convert.ToInt64(Math.Round(result.Order.OrderCalculations.Sum(c => c.Amount) * 100));

        result.PaymentIntent = await _paymentIntentService.CreateAsync(new ()
        {
            Amount = amount,
            Currency = "gbp"
        });

        result.OrderPaymentIntent = new OrderPaymentIntent()
        {
            Order = result.Order,
            PaymentIntentId = result.PaymentIntent.Id,
        };
        db.Add(result.OrderPaymentIntent);
        await db.SaveChangesAsync();

        return result;
    }
}
