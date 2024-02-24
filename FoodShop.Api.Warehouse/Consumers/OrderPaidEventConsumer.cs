using FoodShop.MessageContracts.Order;
using MassTransit;

namespace FoodShop.Api.Warehouse.Consumers;

public class OrderPaidEventConsumer : IConsumer<OrderPaid>
{
    private readonly ILogger<OrderPaidEventConsumer> _logger;

    public OrderPaidEventConsumer(ILogger<OrderPaidEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderPaid> context)
    {
        _logger.LogInformation(context.Message.ToString());
        return Task.CompletedTask;
    }
}
