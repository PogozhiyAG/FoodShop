using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Data;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Order.CommandHandlers;

public class OrderSetStatusPaidCommandHandler(
    IDbContextFactory<OrderDbContext> _dbContextFactory,
    IPublishEndpoint _publishEndpoint
) : IRequestHandler<OrderSetStatusPaidCommand>
{
    public async Task Handle(OrderSetStatusPaidCommand request, CancellationToken cancellationToken)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();

        var orderPaymentIntent = await db.OrderPaymentIntents
            .Include(pi => pi.Order).ThenInclude(o => o.Items)
            .Include(pi => pi.Order).ThenInclude(o => o.DeliveryInfo)
            .FirstOrDefaultAsync(pi => pi.PaymentIntentId == request.PaymentIntentId);

        if (orderPaymentIntent?.Order != null)
        {
            orderPaymentIntent.Order.Status = Model.OrderStatus.Paid;
            await db.SaveChangesAsync();

            await _publishEndpoint.Publish(orderPaymentIntent.Order.MapToOrderPaidMessage());
        }
    }
}


public static class OrderMappingExtensionsTODORefactoring
{
    public static MessageContracts.Order.OrderPaid MapToOrderPaidMessage(this Model.Order order)
    {
        return new MessageContracts.Order.OrderPaid(
            OrderId: order.Id,
            Moment: DateTime.Now
        );
    }
}