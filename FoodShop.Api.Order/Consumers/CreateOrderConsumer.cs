using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Dto;
using FoodShop.MessageContracts.Order;
using MassTransit;
using MediatR;

namespace FoodShop.Api.Order.Consumers;

public class CreateOrderConsumer : IConsumer<CreateOrder>
{
    private readonly ILogger<CreateOrderConsumer> _logger;
    private readonly IMediator _mediator;

    public CreateOrderConsumer(ILogger<CreateOrderConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateOrder> context)
    {
        _logger.LogInformation(context.Message.ToString());

        var command = new CreateCheckoutCommand()
        {
            CreateOrderRequest = context.Message.MapToCreateOrderRequest()
        };

        var commandResult = await _mediator.Send(command);

        var response = new CreateOrderResponse(commandResult.Order!.Id);
        await context.RespondAsync(response);
    }
}



public static class CreateOrderExtensionsTODORefactor
{
    public static CreateOrderRequest MapToCreateOrderRequest(this CreateOrder createOrder)
    {
        var result = new CreateOrderRequest();

        result.Items.AddRange(createOrder.Items.Select(i => new OrderItemDto()
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }));

        var delivery = createOrder.Delivery;
        if(delivery != null)
        {
            result.Delivery = new DeliveryInfoDto()
            {
                Address = delivery.Address,
                ContactName = delivery.ContactName!,
                ContactPhone = delivery.ContactPhone!,
                TimeSlotFrom = delivery.TimeSlotFrom,
                TimeSlotTo = delivery.TimeSlotTo
            };
        }

        result.Description = createOrder.Description;

        return result;
    }
}