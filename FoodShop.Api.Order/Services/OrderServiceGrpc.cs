using FoodShop.Api.Order.Commands;
using FoodShop.Order.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FoodShop.Api.Order.Services;

[Authorize]
public class OrderServiceGrpc : OrderService.OrderServiceBase
{
    private readonly IMediator _mediator;

    public OrderServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<OrderResponse> GetOrder(OrderRequest request, ServerCallContext context)
    {
        var command = new GetOrderCommand()
        {
            OrderId = Guid.Parse(request.OrderId)
        };

        var order = await _mediator.Send(command);

        var result = new OrderResponse()
        {
            Id = order.Id.ToString(),
            UserId = order.UserId
        };

        result.Items.AddRange(order.Items.Select(i => new OrderItem()
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }));

        if(order.DeliveryInfo != null)
        {
            result.DeliveryInfo = new DeliveryInfo()
            {
                Address = order.DeliveryInfo.Address,
                ContactName = order.DeliveryInfo.ContactName,
                ContactPhone = order.DeliveryInfo.ContactPhone,
                TimeSlotFrom = Timestamp.FromDateTime(order.DeliveryInfo.TimeSlotFrom.ToUniversalTime()),
                TimeSlotTo = Timestamp.FromDateTime(order.DeliveryInfo.TimeSlotTo.ToUniversalTime())
            };
        }

        return result;
    }
}
